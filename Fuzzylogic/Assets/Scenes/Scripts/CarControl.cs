using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class CarControl : MonoBehaviour
{
    public FuzzyLogicControl HorizController;
    public FuzzyLogicControl VertlController;

    public Transform LeftPointer, CenterPointer, RightPointer, BackwardPointer;
    public Transform LP_Position, CP_Position, RP_Position, Back_Position;


    public float Scale_Side = 10f;
    public float Scale_Front = 16.0f;
    public float carMaxSpeed = 140f;
    public float carMinSpeed = 80f;
    public float carCurrentSpeed = 80f;
    float constScale = 0.3f;
    float turnSpeed = 1000f;

    float scaleRealRaycastLaterals = 10; float scaleDrawLineRC_Laterals = 10f;
    float scaleRealRaycastFront = 16; float scaleDrawLineRC_Front = 10f;


    RaycastHit hit_Right; RaycastHit hit_Left;
    RaycastHit hit_Back; RaycastHit hit_Center;


    // Start is called before the first frame update
    void Start()
    {
        scaleRealRaycastLaterals = Scale_Side;
        scaleDrawLineRC_Laterals = scaleDrawLineRC_Laterals * constScale;

        HorizController.DistanceHit = Scale_Front;
        HorizController.limitMaximum = 1600f;

        VertlController.DistanceHit = Scale_Side;
        VertlController.limitMaximum = 70;

    }
    // Update is called once per frame

    void FixedUpdate()
    {
        scaleRealRaycastLaterals = Scale_Side; scaleRealRaycastFront = Scale_Front;
        scaleDrawLineRC_Laterals = Scale_Side * constScale; scaleDrawLineRC_Front = Scale_Front * constScale;
        HorizController.DistanceHit = scaleRealRaycastLaterals;


        transform.Translate(Vector3.forward * Time.fixedDeltaTime * carCurrentSpeed);

        // check the raycast of right line and call fuzzy function to determing the turn angle
        if (hit_Right.distance > 0)
        {
            HorizController.DistanceHit = scaleRealRaycastLaterals;
            turnSpeed = HorizController.SetRightinput(hit_Right.distance);
            transform.Rotate(new Vector3(0, turnSpeed * Time.deltaTime, 0)); // negativo rot esque
        }
        // check the raycast of left line and call fuzzy function to determing the turn angle
        if (hit_Left.distance > 0)
        {
            HorizController.DistanceHit = scaleRealRaycastLaterals;
            turnSpeed = HorizController.SetLeftinput(hit_Left.distance);
            transform.Rotate(new Vector3(0, turnSpeed * Time.deltaTime, 0)); // negativo rot direita
        }
        if (carCurrentSpeed < carMinSpeed * 0.5f) carCurrentSpeed = carMinSpeed * 0.5f;
        // check the raycast of center line 
        if (hit_Center.distance > 0)
        {
            VertlController.DistanceHit = scaleRealRaycastFront;
            float temp = VertlController.CrossRuleOf3_Math(carMaxSpeed, VertlController.SetRightinput(hit_Center.distance));
            temp = temp / 1000;
            float prediction = carCurrentSpeed * temp;
            if (prediction >= carMinSpeed) carCurrentSpeed *= temp * Time.fixedDeltaTime;

        }
        else if (hit_Center.distance == 0)
        {
            VertlController.DistanceHit = scaleRealRaycastFront;
            float temp = VertlController.CrossRuleOf3_Math(carMaxSpeed, VertlController.SetLeftinput(hit_Back.distance));
            temp = temp / 1000; if (temp < 0) { temp *= -1; }
            float prediction = carCurrentSpeed * temp;
            if (prediction + carCurrentSpeed <= carMaxSpeed) carCurrentSpeed += prediction * Time.fixedDeltaTime;
        }
        

        // following is the code to draw the raycast line 
        #region LeftUp_Raycast
        if (Physics.Raycast(LeftPointer.position, transform.TransformDirection(LP_Position.localPosition - LeftPointer.localPosition), out hit_Left, scaleRealRaycastLaterals))
        {
            Debug.DrawRay(LeftPointer.position, transform.TransformDirection(LP_Position.localPosition - LeftPointer.localPosition) * scaleDrawLineRC_Laterals, Color.red);
        }
        else
        {
            Debug.DrawRay(LeftPointer.position, transform.TransformDirection(LP_Position.localPosition - LeftPointer.localPosition) * scaleDrawLineRC_Laterals, Color.green);
        }
        #endregion 

        #region RightUp_Raycast
        if (Physics.Raycast(RightPointer.position, transform.TransformDirection(RP_Position.localPosition - RightPointer.localPosition), out hit_Right, scaleRealRaycastLaterals))
        {
            Debug.DrawRay(RightPointer.position, transform.TransformDirection(RP_Position.localPosition - RightPointer.localPosition) * scaleDrawLineRC_Laterals, Color.red);
        }
        else
        {
            Debug.DrawRay(RightPointer.position, transform.TransformDirection(RP_Position.localPosition - RightPointer.localPosition) * scaleDrawLineRC_Laterals, Color.green);
        }
        #endregion
        #region ForwardUp_Raycast
        if (Physics.Raycast(CenterPointer.position, transform.TransformDirection(CP_Position.localPosition - CenterPointer.localPosition), out hit_Center, scaleRealRaycastFront))
        {
            Debug.DrawRay(CenterPointer.position, transform.TransformDirection(CP_Position.localPosition - CenterPointer.localPosition) * scaleDrawLineRC_Front, Color.red);
        }
        else
        {
            Debug.DrawRay(CenterPointer.position, transform.TransformDirection(CP_Position.localPosition - CenterPointer.localPosition) * scaleDrawLineRC_Front, Color.green);
        }
        #endregion

        #region Backward_Raycast
        if (Physics.Raycast(BackwardPointer.position, transform.TransformDirection(Back_Position.localPosition - BackwardPointer.localPosition), out hit_Back, scaleRealRaycastFront))
        {
            Debug.DrawRay(BackwardPointer.position, transform.TransformDirection(Back_Position.localPosition - BackwardPointer.localPosition) * scaleDrawLineRC_Front, Color.red);
        }
        else
        {
            Debug.DrawRay(BackwardPointer.position, transform.TransformDirection(Back_Position.localPosition - BackwardPointer.localPosition) * scaleDrawLineRC_Front, Color.green);
        }
        #endregion
    }
}