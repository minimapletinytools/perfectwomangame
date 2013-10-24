using UnityEngine;
using System.Collections.Generic;



public class CachedFlatParticles
{
	List<FlatElementBase> mParticles = new List<FlatElementBase>();
	
	public bool has_particle()
	{
		return mParticles.Count > 0;
	}
	
	public FlatElementBase take_particle()
	{
		FlatElementBase r = null;
		if(has_particle())
		{
			r = mParticles[mParticles.Count - 1];
			r.Enabled = true;
			mParticles.RemoveAt(mParticles.Count - 1);
		}
		return r;
	}
	
	public void return_particle(FlatElementBase aParticle)
	{
		aParticle.Enabled = false;
		mParticles.Add(aParticle);
	}
}

public class SparkleStarFlashParticle
{
	
	FlatParticleEmitter mEmitter;
	Dictionary<string, CachedFlatParticles> mCachedParticles = new Dictionary<string, CachedFlatParticles>();
	public string[] mParticleTypes = new string[]{"gold","silver","glow","red"};
	
	public SparkleStarFlashParticle()
	{
		mEmitter = new FlatParticleEmitter()
		{
			UseColor = true,
			StartColor = new Color(1,1,1,1),
			EndColor = new Color(1,1,1,0)
		};
		
		foreach(string e in mParticleTypes)
		{
			mCachedParticles[e] = new CachedFlatParticles();
		}
	}
	
	public void create_cached_particles()
	{
	}
	
	
	public void emit_rectangle(float grade, Rect position)
	{
		
	}
	
	public void emit_point(float grade, Vector3 position)
	{
		for(int i = 0; i < 100*grade; i++)
			create_particle("gold",position);
	}
	
	public void create_particle(string aType, Vector3 position)
	{
		//TODO diff particle types
		var cache = mCachedParticles[aType];
		if(!cache.has_particle())
		{
			var newPart = new FlatElementImage(get_color_texture(new Color(1,0,0,1)),new Vector2(50,50),1000);
			foreach (Renderer f in newPart.PrimaryGameObject.GetComponentsInChildren<Renderer>())	
				f.gameObject.layer = ManagerManager.Manager.mBackgroundManager.mBackgroundLayer;
			cache.return_particle(newPart);
		}
		mEmitter.add_particle(cache.take_particle(),position,Random.insideUnitCircle*500,1,"gold");
	}
	
	public void update(float aDelta)
	{
		foreach(var e in mEmitter.update(aDelta))
		{
			//e.destroy();
			//TODO need to identify the type name and put them in the right place
			mCachedParticles[e.id].return_particle(e.element);
		}
	}
	
	
	//point particles 
	//0.9-1 few Golden stars, large
	//0.8-0.9 few golden stars small
	//0.7-0.8 many silver stars + flashy oriented lines
	//0.6-0.7 silver stars
	//0.5-0.6 silver sparks
	//0.3-0.5 red glow??
	//0.0-0.3 red sparks
	
	
	
	
	//TODO move this somewhere else
    static Dictionary<Color, Texture2D> mColorTextures = new Dictionary<Color, Texture2D>();
    public static Texture2D get_color_texture(Color aColor)
    {
        if (!mColorTextures.ContainsKey(aColor))
        {
            int w = 2;
            int h = 2;
            Texture2D rgb_texture = new Texture2D(w, h);
            Color rgb_color = aColor;
            int i, j;
            for (i = 0; i < w; i++)
            {
                for (j = 0; j < h; j++)
                {
                    rgb_texture.SetPixel(i, j, rgb_color);
                }
            }
            rgb_texture.Apply();
            mColorTextures[aColor] = rgb_texture;
        }
        return mColorTextures[aColor];
    }
}


//this doesn't actually use anything from flat other than position...
public class FlatParticleEmitter : FlatElementBase
{
	public class FlatSubParticle
	{
		public string id;
		public Vector3 pos;
		public Vector3 vel;
		public QuTimer timer;
		public FlatElementBase element;
	}
	
	
	LinkedList<FlatSubParticle> mParticles = new LinkedList<FlatSubParticle>();
	
	public FlatParticleEmitter()
	{
		PrimaryGameObject = new GameObject("genDummyFlatPraticleEmitter");
	}
	
	public void initialize_defaults()
	{
		Gravity = new Vector3(0,0,0);
		UseColor = true;
		StartColor = new Color(1,1,1,1);
		EndColor = new Color(1,1,1,1);
	}
	
	public void add_texture_particle(Texture2D aImage, Vector2 aSize, Vector3 aPos, Vector3 aVel, float aLifetime,string aId)
	{
		FlatElementImage image = new FlatElementImage(aImage,aSize,0); //TODO depth
		add_particle(image,aPos,aVel,aLifetime,aId);
	}
	
	public void add_particle(FlatElementBase aParticle, Vector3 aPos, Vector3 aVel, float aLifetime, string aId)
	{
		FlatSubParticle addMe = new FlatSubParticle();
		addMe.id = aId;
		addMe.pos = aPos;
		addMe.vel = aVel;
		addMe.element = aParticle;
		addMe.timer = new QuTimer(0,aLifetime);
		mParticles.AddLast(addMe);
	}
	
	Vector3 Gravity {get; set;}
	
	public bool UseColor {get; set;}
	public Color StartColor {get; set;}
	public Color EndColor {get; set;}
	
	
	public List<FlatSubParticle> update(float aDelta)
	{
		//TODO color and other stuff... 
		//TODO gravity
		
		List<FlatSubParticle> removed = new List<FlatSubParticle>();
		LinkedListNode<FlatSubParticle> starting = mParticles.First;
		while(starting != null)
		{
			LinkedListNode<FlatSubParticle> operating = starting;
			FlatSubParticle e = starting.Value;
			e.pos += e.vel*aDelta;
			e.vel += Gravity*aDelta;
			e.timer.update(aDelta);
			starting = starting.Next;
			if(e.timer.isExpired())
			{
				mParticles.Remove(operating);
				removed.Add(operating.Value);
			}
			
			//update the element
			e.element.HardPosition = e.pos;
			if(UseColor)
				e.element.HardColor = StartColor*(1-e.timer.getLinear()) + EndColor*e.timer.getLinear();
			
			e.element.update(aDelta);
		}
		return removed;
	}
}
