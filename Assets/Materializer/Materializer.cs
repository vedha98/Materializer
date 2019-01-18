using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using System;

public class Materializer : EditorWindow{
	string customPath = "Assets/";
	string Path = "Assets/";
	string MaterialName = "default";
	Shader ShaderToImport;
	String[] shaderNames;
	bool CustomMatEnabled,CustomPathEnabled,MultiMat;
	bool myBool = true;
	Color matColor;
	Color color;
	int tab,matSelect=0,index=0;
	Material mat = null;

	[MenuItem ("Window/Materializer")]

	public static void  ShowWindow () {
		EditorWindow.GetWindow(typeof(Materializer));
	}
	void OnGUI () {
		getShaders ();




		GUILayout.Label ("Materializer", EditorStyles.boldLabel);
		tab = GUILayout.Toolbar (tab, new string[] { "Materialize", "Custom Materials","Colorize"});

		switch (tab) {
		case 2:
			Colorize ();
			break;
		case 0:
			Materialize ();
			break;
		case 1:
			Customize ();
			break;
			default:
			break;
		}



	}

	public void Colorize(){
		

		index = EditorGUILayout.Popup (index, shaderNames);

		color = EditorGUILayout.ColorField ("Color", color);
		if(GUILayout.Button ("apply")){

			foreach (GameObject obj in Selection.gameObjects) {
				Renderer renderer = obj.GetComponent<Renderer> ();
				if(renderer!=null){
					renderer.sharedMaterial.color = color;
				}
			}
		}
	}

	public void Materialize(){
		
		MaterialName = EditorGUILayout.TextField("MaterialName: ", MaterialName);
		index = EditorGUILayout.Popup (index, shaderNames);
		ShaderToImport = Shader.Find (shaderNames [index]);

		for (int i = 0; i < UnityEditor.ShaderUtil.GetPropertyCount (ShaderToImport); i++) {
			
			//EditorGUILayout.PropertyField ();

		}
		matColor = EditorGUILayout.ColorField ("Color", matColor);
		CustomPathEnabled = EditorGUILayout.BeginToggleGroup ("Custom Path", CustomPathEnabled);
		customPath = EditorGUILayout.TextField("Path: ", customPath);

		EditorGUILayout.EndToggleGroup ();
		MultiMat = EditorGUILayout.BeginToggleGroup ("Multiple Materials", MultiMat);
		matSelect = EditorGUILayout.IntField("region: ", matSelect);
		EditorGUILayout.EndToggleGroup ();

		if(GUILayout.Button ("apply")){

			if (CustomPathEnabled) {
				Path = customPath+"/";
				if (!AssetDatabase.IsValidFolder (customPath)) {
					string parent = "Assets";
					string[] ssizes = Path.Split('/');
					foreach (string s in ssizes) {
						if (s != "" && s != " " && s!="Assets") {
							AssetDatabase.CreateFolder (parent, s);
							parent = parent + "/" + s;
						}

					}

				}





			} else {
				Path = "Assets/";
			}
			Material material = new Material(ShaderToImport);
			material.color = matColor;
			Material test = null;
			test = (Material)AssetDatabase.LoadAssetAtPath(Path+MaterialName+".mat", typeof(Material));
			if (test != null) {
				Debug.Log ("Material already exists! so the material was not saved");
			} else {
				AssetDatabase.CreateAsset(material, Path+MaterialName+".mat");
			}
				



			foreach (GameObject obj in Selection.gameObjects) {
				Renderer renderer = obj.GetComponent<Renderer> ();
				if(renderer!=null){
					if (MultiMat) {
						Material[] mats = renderer.sharedMaterials;
						try{
							mats [matSelect-1] = material;
						}catch(Exception exception){
							
						};

						renderer.sharedMaterials = mats;
					} else {
						renderer.sharedMaterial = material;
					}



				}
			}
		}
	}

	public void Customize(){




		EditorGUILayout.BeginHorizontal();
		mat =(Material) EditorGUILayout.ObjectField(mat, typeof(Material), true);
		EditorGUILayout.EndHorizontal();
		if(GUILayout.Button ("apply")){
			foreach (GameObject obj in Selection.gameObjects) {
				Renderer renderer = obj.GetComponent<Renderer> ();
				if(renderer!=null){
					renderer.sharedMaterial = mat;
				}
			}
		}
	}

	public void getShaders(){

		ShaderInfo[] so = UnityEditor.ShaderUtil.GetAllShaderInfo ();
		shaderNames = new string[so.Length];
		int i = 0;
		foreach (ShaderInfo ss in so) {
			if (!ss.hasErrors) {
				shaderNames [i] = ss.name;
				i++;
			}


		}
	}

}
#endif