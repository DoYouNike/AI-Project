using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
public class FuzzyLogicControl : MonoBehaviour
{

    public AnimationCurve FuzzyDir_TooIntoRight;
    public AnimationCurve FuzzyDir_Right;
    public AnimationCurve FuzzyDir_BarelyRight;

    public AnimationCurve FuzzyDir_TooIntoLeft;
    public AnimationCurve FuzzyDir_Left;
    public AnimationCurve FuzzyDir_BarelyLeft;

    public AnimationCurve FuzzyOutput_Right;
    public AnimationCurve FuzzyOutput_Middle;
    public AnimationCurve FuzzyOutputLeft;

    
    private Dictionary<string, float> Membership_Right;
    private Dictionary<string, float> Membership_Left;
    private Dictionary<string, float> Membership_Output;

    //create fuzzy rule tables
    public Dictionary<string, string> RuleBase;

    
    private float Output;

  
    public float InputValueRight;
    public float InputValueLeft;

    //this function set the value to each turn state 
    public void FuzzyRight(float inputValue)
    {
        Membership_Right["HardRight"] = FuzzyDir_TooIntoRight.Evaluate(inputValue);
        Membership_Right["NeutralRight"] = FuzzyDir_Right.Evaluate(inputValue);
        Membership_Right["BarelyRight"] = FuzzyDir_BarelyRight.Evaluate(inputValue);
    }

    public void FuzzyLeft(float inputValue)
    {
        Membership_Left["HardLeft"] = FuzzyDir_TooIntoLeft.Evaluate(inputValue);
        Membership_Left["NeutralLeft"] = FuzzyDir_Left.Evaluate(inputValue);
        Membership_Left["BarelyLeft"] = FuzzyDir_BarelyLeft.Evaluate(inputValue);
    }

    
    public void SetFuzzy(Vector2[] keyframes, AnimationCurve Curve)
    {
        int frame = 0;
        foreach (var key in keyframes)
        {
            Curve.AddKey(key.x, key.y);
        }

        foreach (var key in keyframes)
        {
            AnimationUtility.SetKeyLeftTangentMode(Curve, frame, AnimationUtility.TangentMode.Linear);
            AnimationUtility.SetKeyRightTangentMode(Curve, frame, AnimationUtility.TangentMode.Linear);
            frame += 1;
        }
    }

    //delete any value that is not needed
    public void ZeroMemberships()
    {

        Membership_Output["Hefty"] = 0f;
        Membership_Output["Moderate"] = 0f;
        Membership_Output["Primary"] = 0f;

        Membership_Right["HardRight"] = 0f;
        Membership_Right["NeutralRight"] = 0f;
        Membership_Right["BarelyRight"] = 0f;

        Membership_Left["HardLeft"] = 0f;
        Membership_Left["NeutralLeft"] = 0f;
        Membership_Left["BarelyLeft"] = 0f;
    }

    
    public void EvaluateRuleBase()
    {
        foreach (var keyA in Membership_Right.Keys)
        {
            foreach (var keyF in Membership_Left.Keys)
            {
                if (Membership_Left[keyF] > -600 && Membership_Right[keyA] > -600)
                {
                    if (Membership_Left[keyF] < 600 && Membership_Right[keyA] < 600)
                    {
                        string Group = RuleBase[keyF + keyA];
                        Membership_Output[Group] = Mathf.Max(Mathf.Min(Membership_Right[keyA], Membership_Left[keyF]), Membership_Output[Group]);

                    }
                }
            }
        }

    }


    // This function calcuates the are of half of the triangle
    private float CalculateHalfArea(float x0, float x1, float x2, float x3, float u)
    {
        
        if (x1 == x3)
            return 0;

        float m = (x1 - x3) / (x0 - x2);
        float b = x3 - m * x2;
        float xu = (u - b) / m;
        float area = 0;
        if (m < 0)
            area = u * (x2 - x0 + xu - x0) / 2;
        else
            area = u * (x2 - x0 + x2 - xu) / 2;

        return area;

    }

    //Divide the membership into two rectangle triangles and sum up their areas
    private float CalculateTrapezoidArea(AnimationCurve function, float U)
    {
       

        float areaA = CalculateHalfArea(function.keys[0].time, function.keys[0].value, function.keys[1].time, function.keys[1].value, U);
        float areaB = CalculateHalfArea(function.keys[1].time, function.keys[1].value, function.keys[2].time, function.keys[2].value, U);

        return areaA + areaB;
    }


    //Return the maximum point of the Curve
    private float CalculateCenter(AnimationCurve function, float U)
    {
        return (function.keys[1].time);
    }

    
    public void DeFuzzy()
    {
        EvaluateRuleBase();

        List<float> Areas = new List<float>();
        List<float> Centers = new List<float>();

        // For each possible membership, calculate the area and its center
        foreach (var keyP in Membership_Output.Keys)
        {
            float U_a = Membership_Output[keyP];

            float area = 0;
            float center = 0;
            if (keyP == "Hefty")
            {
                area = CalculateTrapezoidArea(FuzzyOutput_Right, U_a);
                center = CalculateCenter(FuzzyOutput_Right, U_a);
            }
            else if (keyP == "Moderate")
            {
                area = CalculateTrapezoidArea(FuzzyOutput_Middle, U_a);
                center = CalculateCenter(FuzzyOutput_Middle, U_a);
            }
            else
            {
                area = CalculateTrapezoidArea(FuzzyOutputLeft, U_a);
                center = CalculateCenter(FuzzyOutputLeft, U_a);
            }
            Areas.Add(area);
            Centers.Add(center);
        }

        float numerator = 0;
        float den = 0;

   
        for (int i = 0; i < Areas.Count; i++)
        {
            numerator += Areas[i] * Centers[i];
            den += Areas[i];
        }

        Output = numerator / den;
    }

    public float DistanceHit = 12f; 
    public float limitMaximum = 1600f;
  
    void Awake()
    {

        // Set values for each membership function
        SetFuzzy(new[] { new Vector2(0f, 1f), new Vector2((DistanceHit * 0.8f), 0.8f), new Vector2(DistanceHit, 0f) }, FuzzyDir_TooIntoRight);
        SetFuzzy(new[] { new Vector2(0f, 0f), new Vector2(DistanceHit, 1f), new Vector2((DistanceHit * 2), 0) }, FuzzyDir_Right);
        SetFuzzy(new[] { new Vector2(0f, 0f), new Vector2(DistanceHit, 1f) }, FuzzyDir_BarelyRight);

        SetFuzzy(new[] { new Vector2(0f, 1f), new Vector2((DistanceHit * 0.8f), 0.8f), new Vector2(DistanceHit, 0f) }, FuzzyDir_TooIntoLeft);
        SetFuzzy(new[] { new Vector2(0f, 0f), new Vector2(DistanceHit, 1f), new Vector2((DistanceHit * 2), 0) }, FuzzyDir_Left);
        SetFuzzy(new[] { new Vector2(0f, 0f), new Vector2(DistanceHit, 1f) }, FuzzyDir_BarelyLeft);

        SetFuzzy(new[] { new Vector2(-limitMaximum, 0f), new Vector2(-(limitMaximum * 0.625f), 1f), new Vector2(0f, 0f) }, FuzzyOutputLeft);
        SetFuzzy(new[] { new Vector2(-(limitMaximum * 0.375f), 0f), new Vector2(0, 1f), new Vector2((limitMaximum * 0.375f), 0) }, FuzzyOutput_Middle);
        SetFuzzy(new[] { new Vector2(0f, 0f), new Vector2((limitMaximum * 0.625f), 1f), new Vector2(limitMaximum, 0f) }, FuzzyOutput_Right);


        //Initialize RuleBase dictionary and fill in the rules
        RuleBase = new Dictionary<string, string>();

        RuleBase.Add("HardLeftHardRight", "Moderate");
        RuleBase.Add("HardLeftNeutralRight", "Hefty");
        RuleBase.Add("HardLeftBarelyRight", "Hefty");

        RuleBase.Add("NeutralLeftHardRight", "Primary");
        RuleBase.Add("NeutralLeftNeutralRight", "Moderate");
        RuleBase.Add("NeutralLeftBarelyRight", "Hefty");

        RuleBase.Add("BarelyLeftHardRight", "Primary");
        RuleBase.Add("BarelyLeftNeutralRight", "Primary");
        RuleBase.Add("BarelyLeftBarelyRight", "Moderate");

        //Intialize membership values
        Membership_Right = new Dictionary<string, float>();
        Membership_Left = new Dictionary<string, float>();
        Membership_Output = new Dictionary<string, float>();

        Membership_Output.Add("Hefty", 0f);
        Membership_Output.Add("Moderate", 0f);
        Membership_Output.Add("Primary", 0f);

        Membership_Right.Add("HardRight", 0f);
        Membership_Right.Add("NeutralRight", 0f);
        Membership_Right.Add("BarelyRight", 0f);

        Membership_Left.Add("HardLeft", 0f);
        Membership_Left.Add("NeutralLeft", 0f);
        Membership_Left.Add("BarelyLeft", 0f);

        InputValueRight = 0;
        InputValueLeft = 0;

    }
    // function to be  used in car control to determine the turn state
    public float SetLeftinput(float inputAlignmentLEFT)
    {
        InputValueLeft = inputAlignmentLEFT;
        return InitFuzzy();
    }
    public float SetRightinput(float inputAligmentRIGHT)
    {
        InputValueRight = inputAligmentRIGHT;
        return InitFuzzy();
    }
    float InitFuzzy()
    { 
        ZeroMemberships();
 
        FuzzyRight(InputValueRight);
        FuzzyLeft(InputValueLeft);
 
        EvaluateRuleBase();

        DeFuzzy();

        return Output;
    }

    public float CrossRuleOf3_Math(float fullv, float piecev)//multiplies crossed to find a vlue of some percentage
    {
        float fullvalue; float fullpercent = 100f;
        float piecevalue; float x;

        fullvalue = fullv;
        piecevalue = piecev;

        x = piecevalue * fullpercent;
        x = x / fullvalue;

        return x;
    }
}


