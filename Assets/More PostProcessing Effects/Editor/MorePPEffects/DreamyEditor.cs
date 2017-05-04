﻿/*
 * Author : Maxime JUMELLE
 * Namespace : MorePPEffects
 * Project : More Post-Processing Effects Package
 * 
 * If you have any suggestion or comment, you can write me at webmaster[at]hardgames3d.com
 * 
 * File : DreamyEditor.cs
 * 
 * */

using UnityEngine;
using System.Collections;
using UnityEditor;
using MorePPEffects;

namespace MorePPEffects
{
	[CustomEditor(typeof(Dreamy))]
	class DreamyEditor : Editor {

		private SerializedObject s_target;
		private SerializedProperty s_strength;

		public void OnEnable()
		{
			s_target = new SerializedObject (target);
			s_strength = s_target.FindProperty ("strength");
		}

		public override void OnInspectorGUI()
		{
			s_target.Update ();

			EditorGUILayout.LabelField ("Recreates a dream effect.", EditorStyles.miniLabel);
			EditorGUILayout.PropertyField (s_strength, new GUIContent("Strength"));
			
			s_target.ApplyModifiedProperties();
		}
	}
}