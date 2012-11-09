using UnityEngine;
using System.Collections;

//solves ma = -xk + bv
public class QuHarmonicOscillator {
	float k,b,m,t,x,v;
	QuHarmonicOscillator(float aX, float aK, float aM = 0,float aB = 0)
	{
		x = aX;
		t = aX;
		m = aM;
		k = aK;
		b = aB;
		v = 0;
	}
	
	public void set_target(float aTarget){t = aTarget;}
	public void add_impulse(float impulse){v += impulse/m;}
	public void update(float dt) //discrete, I'm too stupid to do the analytic solution
	{
		float det = (k/m*dt + 1 - b/m);
		x = -dt*v/det;
		v = (1-b/m)/det;
	}
}
