using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Text;
using System.IO;

public sealed class CountLines: EditorWindow 
{
	private StringBuilder strStats = new StringBuilder();
	private Vector2 scrollPosition = new Vector2(0, 0);

	struct File
	{
		public string name;
		public int nbLines;
		
		public File(string name, int nbLines)
		{
			this.name = name;
			this.nbLines = nbLines;
		}
	}	
	
	private void OnGUI()
	{
		if (GUILayout.Button("Refresh"))
			DoCountLines();

		scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
		EditorGUILayout.HelpBox(strStats.ToString(), MessageType.None);
		EditorGUILayout.EndScrollView();
	}
	
	[MenuItem("Tools/Count Code Lines")]
	public static void Init()
	{
		CountLines window = EditorWindow.GetWindow<CountLines>("Count Lines");
		window.Show();
		window.Focus();
		window.DoCountLines();
	}
	
	private void DoCountLines()
	{		
		string strDir = Directory.GetCurrentDirectory();
		strDir += @"/Assets/Code";

		int iLengthOfRootPath = strDir.Length;

		ArrayList stats = new ArrayList();	
		ProcessDirectory(stats, strDir);	
		
		int iTotalNbLines = 0;

		foreach (File f in stats)
			iTotalNbLines += f.nbLines;
		
		strStats = new StringBuilder();
		strStats.Append("Number of Files: " + stats.Count + "\n");		
		strStats.Append("Number of Lines: " + iTotalNbLines + "\n");	
		strStats.Append("================\n");	
		
		foreach (File f in stats)
			strStats.Append(f.name.Substring(iLengthOfRootPath + 1, f.name.Length - iLengthOfRootPath - 1) + " --> " + f.nbLines + "\n");
	}
	
	private static void ProcessDirectory(ArrayList stats, string dir)
	{	
		string[] strArrFiles = Directory.GetFiles(dir, "*.cs");

		foreach (string strFileName in strArrFiles)
			ProcessFile(stats, strFileName);
		
		strArrFiles = Directory.GetFiles(dir, "*.js");

		foreach (string strFileName in strArrFiles)
			ProcessFile(stats, strFileName);
		
		string[] strArrSubDir = System.IO.Directory.GetDirectories(dir);

		foreach (string strSubDir in strArrSubDir)
			ProcessDirectory(stats, strSubDir);
	}
	
	private static void ProcessFile(ArrayList stats, string filename)
	{
		StreamReader reader = System.IO.File.OpenText(filename);
		int iLineCount = 0;
		
		while (reader.Peek() >= 0)
		{
			reader.ReadLine();
			++iLineCount;
		}
		
		stats.Add(new File(filename, iLineCount));
		reader.Close();			
	}
}