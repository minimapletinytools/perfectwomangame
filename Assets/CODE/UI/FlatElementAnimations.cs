using UnityEngine;
using System.Collections;

public class FlatElementAnimations {
    public delegate bool ElementAnimationDelegate(FlatElementBase aElement, float aDeltaTime);
    public delegate float ForceFunctionDelegate(float aTime);
    public delegate void SetForceDelegate(float aTime, float aForce, FlatElementBase aElement);

    public static void SetPosition(float aTime, float aForce, FlatElementBase aElement)
    {
        aElement.mLocalPosition = Random.insideUnitCircle * aForce;
    }
    public static void SetRotation(float aTime, float aForce, FlatElementBase aElement)
    {
        aElement.mLocalRotation = Quaternion.AngleAxis(aForce * Random.Range(-1.0f, 1.0f), Vector3.forward);
    }
    public static void SetColor(float aTime, float aForce, FlatElementBase aElement)
    {
        aElement.mLocalColor = aForce * (new Color(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f)));
    }
    
    public float One(float aTime) { return 1; }

    public class GenericAnimation
    {
        QuTimer mTime;
        float mForce;
        SetForceDelegate mChange;
        ForceFunctionDelegate mFunction;
        public GenericAnimation(float aTime, float aForce, SetForceDelegate aChange, ForceFunctionDelegate aFunction)
        {
            mTime = new QuTimer(0, aTime);
            mForce = aForce;
            mChange = aChange;
        }
        public bool animate(FlatElementBase aElement, float aDeltaTime)
        {
            mTime.update(aDeltaTime);
            mChange(mTime.getLinear(), mFunction(mTime.getLinear())*mForce, aElement);
            return mTime.isExpired();
        }
    }

    public ElementAnimationDelegate position_jiggle_delegate(float aTime, float aForce)
    {
        return (new GenericAnimation(aTime, aForce, SetPosition, One)).animate;
    }
    public ElementAnimationDelegate color_jiggle_delegate(float aTime, float aForce)
    {
        return (new GenericAnimation(aTime, aForce, SetColor, One)).animate;
    }
    public ElementAnimationDelegate rotation_jiggle_delegate(float aTime, float aForce)
    {
        return (new GenericAnimation(aTime, aForce, SetRotation, One)).animate;
    }


    public class FloatingAnimation
    {
        //TODO
    }
}
