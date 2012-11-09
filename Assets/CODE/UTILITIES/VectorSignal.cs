using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class VectorSignal {
	public class Pair
	{
		public Pair(Vector3 aValue, float aTime){mValue = aValue; mTime = aTime;}
		public Vector3 mValue;
		public float mTime;
	}
	List<Pair> mValues = new List<Pair>();
	public VectorSignal(Vector3 start, float time = 0){mValues.Add(new Pair(start,time));}
	public VectorSignal(){}
	public bool has_values(){return mValues.Count != 0;}
	
	public void add_absolute(Vector3 v, float t)
	{
		if(mValues.Count != 0 && mValues[mValues.Count-1].mTime >= t)
			throw new UnityException("QuSignal must be strictly monotonic in time " + mValues[mValues.Count-1].mTime + " " + t);
		mValues.Add(new Pair(v,t));
	}
	public void add_relative(Vector3 v, float t)
	{
		if(t <= 0)
			throw new UnityException("QuSignal must be strictly monotonic in time");
		mValues.Add(new Pair(v,mValues[mValues.Count-1].mTime + t));
	}
	public Vector3 get_last(){return mValues[mValues.Count-1].mValue;}
	public Vector3 get_first(){return mValues[0].mValue;}
	public float get_total_time(){return mValues[mValues.Count-1].mTime - mValues[0].mTime;}
	public Vector3 get_change(){return get_last() - get_first();}
	public Vector3 get_average_velocity(){return get_change()/get_total_time();}
	public Vector3 get_last_value_difference(){return get_last() - mValues[Mathf.Max(0,mValues.Count-2)].mValue;}
	public float get_last_time_change(){return mValues[mValues.Count-1].mTime - mValues[Mathf.Max(0,mValues.Count-2)].mTime;}
	//public T get_average_velocity(float support)
}
