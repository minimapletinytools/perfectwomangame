
using UnityEngine;
using UnityEditor;
using System.IO;
public class BuildScripts
{
    [MenuItem("Custom/build/OSX")]
    static void build_osx()
    {
		string[] scenes = {"Assets/SCENES/kinect_test.unity"};
 
		string buildDir = "/Users/user/Desktop/unitybuilds/lea/" + System.DateTime.Now.ToString("MMMdhmm") +"_osx.app";
		System.IO.Directory.CreateDirectory(buildDir);
		BuildPipeline.BuildPlayer(scenes , buildDir, BuildTarget.StandaloneOSXIntel, BuildOptions.None);
		
		string resourceDstPath = buildDir + "/Contents/Resources";
		DirectoryCopy(Application.dataPath + "/Resources", resourceDstPath,false);
    }
	
	[MenuItem("Custom/build/WIN")]
    static void build_w()
    {
		string[] scenes = {"Assets/SCENES/kinect_test.unity"};
 
		string buildDir = "/Users/user/Desktop/unitybuilds/lea/" + System.DateTime.Now.ToString("MMMdhmm") + "/PW.exe";
		System.IO.Directory.CreateDirectory(buildDir);
		BuildPipeline.BuildPlayer(scenes , buildDir, BuildTarget.StandaloneWindows, BuildOptions.None);
		
		string resourceDstPath = buildDir + "_data/Resources";
		DirectoryCopy(Application.dataPath + "/Resources", resourceDstPath,false);
    }
	
	private static void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs)
    {
        DirectoryInfo dir = new DirectoryInfo(sourceDirName);
        DirectoryInfo[] dirs = dir.GetDirectories();

        if (!dir.Exists)
        {
            throw new DirectoryNotFoundException(
                "Source directory does not exist or could not be found: "
                + sourceDirName);
        }

        if (!Directory.Exists(destDirName))
        {
            Directory.CreateDirectory(destDirName);
        }

        FileInfo[] files = dir.GetFiles();
        foreach (FileInfo file in files)
        {
            string temppath = Path.Combine(destDirName, file.Name);
            file.CopyTo(temppath, false);
        }

        if (copySubDirs)
        {
            foreach (DirectoryInfo subdir in dirs)
            {
                string temppath = Path.Combine(destDirName, subdir.Name);
                DirectoryCopy(subdir.FullName, temppath, copySubDirs);
            }
        }
    }
  
}