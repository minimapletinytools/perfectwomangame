using UnityEngine;
using System.Collections;
using System.Reflection;
using System;

public class FakeMonoBehaviour {
	
	public ManagerManager mManager;
	public FakeMonoBehaviour(ManagerManager aManager)
	{
		mManager = aManager;
		mManager.register_FakeMonoBehaviour(this);
	}
	
	~FakeMonoBehaviour()
	{
		mManager.deregister_FakeMonoBehaviour(this);
	}
	
	
	public virtual void Start () {
	}
	public virtual void Update () {
	}
	public virtual void FixedUpdate(){
	}
	
	
	public bool is_method_overridden(string methodName)
	{
		var t = this.GetType();
		var mi = t.GetMethod(methodName, BindingFlags.Instance | BindingFlags.Public);
		if (mi == null) return false;
		var declaringType = mi.DeclaringType.FullName;
		return declaringType.Equals(t.FullName, StringComparison.OrdinalIgnoreCase);
	}
}
