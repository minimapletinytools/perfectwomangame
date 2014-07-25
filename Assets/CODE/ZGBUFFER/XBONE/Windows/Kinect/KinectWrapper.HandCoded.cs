using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

public class MonoPInvokeCallbackAttribute : System.Attribute
{
	public System.Type RootType { get; set; }
	public MonoPInvokeCallbackAttribute(System.Type t) { RootType = t; }
}

namespace Windows.Foundation
{
    public struct Point
    {
    }
}

namespace Helper
{
    public static class NativeObjectCache
    {
        private static object _lock = new object();
        private static Dictionary<Type, Dictionary<IntPtr, WeakReference>> _objectCache = new Dictionary<Type, Dictionary<IntPtr, WeakReference>>();
        
        public static IntPtr MapToIUnknown(IntPtr nativePtr)
        {
            //private static Guid _guidIUnknown = new Guid("00000000-0000-0000-C000-000000000046");
            // NOTE: The IntPtr needs to use the IUnknown identity
            return nativePtr;
        }
        
        public static void AddObject<T>(IntPtr nativePtr, T obj) where T : class
        {
            lock (_lock)
            {
                Dictionary<IntPtr, WeakReference> objCache = null;
                
				if (!_objectCache.TryGetValue(typeof(T), out objCache) || objCache == null)
                {
                    objCache = new Dictionary<IntPtr, WeakReference>();
                    _objectCache[typeof(T)] = objCache;
                }
                
                objCache[nativePtr] = new WeakReference (obj);
            }
        }
        
        public static void RemoveObject<T>(IntPtr nativePtr)
        {
            lock (_lock)
            {
                Dictionary<IntPtr, WeakReference> objCache = null;
                
				if (!_objectCache.TryGetValue(typeof(T), out objCache) || objCache == null)
                {
                    objCache = new Dictionary<IntPtr, WeakReference>();
                    _objectCache[typeof(T)] = objCache;
                }
                
                if (objCache.ContainsKey(nativePtr))
                {
                    objCache.Remove(nativePtr);
                }
            }
        }
        
        public static T GetObject<T>(IntPtr nativePtr) where T : class
        {
            lock (_lock) 
            {
                Dictionary<IntPtr, WeakReference> objCache = null;
                
                if (!_objectCache.TryGetValue(typeof(T), out objCache) || objCache == null)
                {
                    objCache = new Dictionary<IntPtr, WeakReference>();
                    _objectCache[typeof(T)] = objCache;
                }
                
                WeakReference reference = null;
                if (objCache.TryGetValue(nativePtr, out reference)) 
                {
                	if(reference != null)
                	{
	                    T obj = reference.Target as T; 
	                    if(obj != null)
	                    {
	                        return (T)obj;
	                    }
                    }
                }
                
                return null;
            }
        }
    }
}

namespace Windows.Kinect
{
    // Here is the delegate for the managed callback
	internal delegate void PropertyChanged(
		IntPtr pNative,
		[MarshalAs(UnmanagedType.LPWStr)] string propertyName);
	
	public partial class KinectSensor : System.ComponentModel.INotifyPropertyChanged
	{
		[DllImport("KinectForUnity")]
		private static extern void Windows_Kinect_KinectSensor_RegisterPropertyChangedCallback(
			IntPtr pNative,
			PropertyChanged pCallback);
			
		private object _eventLock = new Object();
		private event System.ComponentModel.PropertyChangedEventHandler _PropertyChanged;
		public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged
		{
			add
			{
				lock(_eventLock)
				{
				 	if (_PropertyChanged == null)
				 	{
						Windows_Kinect_KinectSensor_RegisterPropertyChangedCallback(_pNative, KinectSensor.OnPropertyChanged);
				 	}
				 	
					_PropertyChanged += value;
				}
			}
			
			remove
			{
				lock(_eventLock)
                {
					_PropertyChanged -= value;
					
					if (_PropertyChanged == null)
					{
						Windows_Kinect_KinectSensor_RegisterPropertyChangedCallback(_pNative, null);
                    }
                }
			}
		}
		
		[MonoPInvokeCallbackAttribute(typeof(PropertyChanged))]
		private static void OnPropertyChanged(IntPtr pNative, string propertyName)
		{
			KinectSensor sensor = Helper.NativeObjectCache.GetObject<KinectSensor>(pNative);
			
			if (sensor != null)
			{
				if (sensor._PropertyChanged != null)
				{
					sensor._PropertyChanged(sensor, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
				}
			}
		}
	}
		
    public partial class Body
    {
        internal Body() 
        {
            _pNative = IntPtr.Zero;
        }
        
        public IntPtr GetNativePointer() { return _pNative; }
        public void SetNativePointer(IntPtr pNative)
        {
            _pNative = pNative;
            /*_joints = null;
            _activities = null;
            _appearance = null;
            _expressions = null;
            _jointorientations = null;*/
        }
        
        /*
        [System.Runtime.InteropServices.DllImport("KinectForUnity")]
        private static extern void Windows_Kinect_Body_get_Joints(System.IntPtr pNative, Joint[] joints, int jointCount);
        private Dictionary<JointType, Joint> _joints = null;
        public IDictionary<JointType, Joint> Joints
        {
            get
            {
                if(_joints == null)
                {
                    Joint[] joints =  new Joint[JointCount];
                    
                    Windows_Kinect_Body_get_Joints(_pNative, joints, joints.Length);
                    
                    _joints = new Dictionary<JointType, Joint>();
                    for (int index = 0; index < joints.Length; index++)
                    {
                        _joints[(JointType)index] = joints[index];
                    }
                }
                
                return _joints;
            }
        }
        
        [System.Runtime.InteropServices.DllImport("KinectForUnity")]
        private static extern void Windows_Kinect_Body_get_Activities(System.IntPtr pNative, DetectionResult[] results, int resultCount);
        private Dictionary<Activity, DetectionResult> _activities = null;
        public IDictionary<Activity, DetectionResult> Activities
        {
            get
            {
                if(_activities == null)
                {
                    DetectionResult[] results = new DetectionResult[(int)Activity.LookingAway + 1];
                    Windows_Kinect_Body_get_Activities(_pNative, results, results.Length);
                    
                    _activities = new Dictionary<Activity, DetectionResult>();
                    for (int index = 0; index < results.Length; index++)
                    {
                        _activities[(Activity)index] = results[index];
                    }
                }
                
                return _activities;
            }
        }
        
        [System.Runtime.InteropServices.DllImport("KinectForUnity")]
        private static extern void Windows_Kinect_Body_get_Appearance(System.IntPtr pNative, DetectionResult[] results, int resultCount);
        private Dictionary<Appearance, DetectionResult> _appearance = null;
        public IDictionary<Appearance, DetectionResult> Appearance
        {
            get
            {
                if(_appearance == null)
                {
                    DetectionResult[] results = new DetectionResult[(int)Windows.Kinect.Appearance.WearingGlasses + 1];
                    Windows_Kinect_Body_get_Appearance(_pNative, results, results.Length);
                    
                    _appearance = new Dictionary<Appearance, DetectionResult>();
                    for (int index = 0; index < results.Length; index++)
                    {
                        _appearance[(Appearance)index] = results[index];
                    }
                }
                
                return _appearance;
            }
        }
        
        [System.Runtime.InteropServices.DllImport("KinectForUnity")]
        private static extern void Windows_Kinect_Body_get_Expressions(System.IntPtr pNative, DetectionResult[] results, int resultCount);
        private Dictionary<Expression, DetectionResult> _expressions = null;
        public IDictionary<Expression, DetectionResult> Expressions
        {
            get
            {
                if(_expressions == null)
                {
                    DetectionResult[] results = new DetectionResult[(int)Expression.Happy + 1];
                    Windows_Kinect_Body_get_Expressions(_pNative, results, results.Length);
                    
                    _expressions = new Dictionary<Expression, DetectionResult>();
                    for (int index = 0; index < results.Length; index++)
                    {
                        _expressions[(Expression)index] = results[index];
                    }
                }
                
                return _expressions;
            }
        }
        
        [System.Runtime.InteropServices.DllImport("KinectForUnity")]
        private static extern void Windows_Kinect_Body_get_JointOrientations(System.IntPtr pNative, JointOrientation[] results, int resultCount);
        private Dictionary<JointType, JointOrientation> _jointorientations = null;
        public IDictionary<JointType, JointOrientation> JointOrientations
        {
            get
            {
                if(_jointorientations == null)
                {
                    JointOrientation[] results = new JointOrientation[JointCount];
                    Windows_Kinect_Body_get_JointOrientations(_pNative, results, results.Length);
                    
                    _jointorientations = new Dictionary<JointType, JointOrientation>();
                    for (int index = 0; index < results.Length; index++)
                    {
                        _jointorientations[(JointType)index] = results[index];
                    }
                }
                
                return _jointorientations;
            }
        }
        */
    }
    
    public partial class BodyFrame
    {
        public void GetAndRefreshBodyData(Body[] bodies)
        {
            IntPtr[] bodyPtr = new IntPtr[bodies.Length];
            for(int x = 0;x<bodies.Length;x++)
            {
            	if (bodies[x] == null)
            	{
            		bodies[x] = new Body();
            	}
            	
                bodyPtr[x] = bodies[x].GetNativePointer();
            }
            
			Windows_Kinect_BodyFrame_GetAndRefreshBodyData(_pNative, bodyPtr, bodies.Length);
            
            for(int x = 0;x<bodies.Length;x++)
            {
                bodies[x].SetNativePointer(bodyPtr[x]);
            }
        }
    }
    
	/*public partial class ColorFrame
	{
		[System.Runtime.InteropServices.DllImport("KinectForUnity")]
		private static extern void Windows_Kinect_ColorFrame_CopyFrameDataToTexture(System.IntPtr pNative, System.IntPtr pNativeTexture);
		public void CopyFrameDataToTexture(UnityEngine.Texture2D texture)
		{
			if (_pNative == System.IntPtr.Zero)
			{
				throw new System.ObjectDisposedException("ColorFrame");
			}
			
			System.IntPtr pNativeTexture = texture.GetNativeTexturePtr();
			if (pNativeTexture == System.IntPtr.Zero)
			{
				return;
			}
			
			Windows_Kinect_ColorFrame_CopyFrameDataToTexture(_pNative, pNativeTexture);
		}
	}*/
}