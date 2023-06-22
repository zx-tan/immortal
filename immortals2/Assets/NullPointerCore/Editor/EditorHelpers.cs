using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace NullPointerEditor
{
	public class EditorHelpers
	{

		public static T GetPropertySource<T>(SerializedProperty prop)
		{
			// Separate the steps it takes to get to this property
			string[] separatedPaths = prop.propertyPath.Split('.');

			// Go down to the root of this serialized property
			System.Object reflectionTarget = prop.serializedObject.targetObject as object;
			// Walk down the path to get the target object
			foreach (var path in separatedPaths)
			{
				FieldInfo fieldInfo = reflectionTarget.GetType().GetField(path, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
				reflectionTarget = fieldInfo.GetValue(reflectionTarget);
			}
			return (T)reflectionTarget;
		}

		public static List<Type> CollectAvailableComponents<C>()
		{
			Type target = typeof(C);
			return CollectAvailableComponents(target);
		}

		public static List<Type> CollectAvailableComponents(Type target)
		{
			List<Type> result = new List<Type>();

			List<Type> listOfTypes = (from domainAssembly in AppDomain.CurrentDomain.GetAssemblies()
								  from assemblyType in domainAssembly.GetTypes()
									  //where typeof(GameEntityComponent).IsAssignableFrom(assemblyType)
								  where target.IsAssignableFrom(assemblyType)
								  select assemblyType).ToList();

			listOfTypes.Add(target);

			// First, we are going to do some filtering here
			foreach (Type type in listOfTypes)
			{
				if (Attribute.GetCustomAttribute(type, typeof(ObsoleteAttribute)) != null)
					continue;
				if (type.IsAbstract)
					continue;
				result.Add(type);
			}
			return result;
		}

		public static IEnumerable<Type> CollectAvailableComponents<T,U>(IEnumerable<T> objectTargets) where T : Component where U : Component
		{
			List<Type> result = new List<Type>();

			Type [] listOfTypes = (from domainAssembly in AppDomain.CurrentDomain.GetAssemblies()
                   from assemblyType in domainAssembly.GetTypes()
                   //where typeof(GameEntityComponent).IsAssignableFrom(assemblyType)
				   where assemblyType.IsSubclassOf(typeof(U))
                   select assemblyType).ToArray();
			
			// First, we are going to do some filtering here
			foreach(Type type in listOfTypes)
			{
				if(Attribute.GetCustomAttribute(type, typeof(ObsoleteAttribute))!=null)
					continue;
				if( EditorHelpers.IsPresentAlongWith<T>(objectTargets, type) )
					continue;
				result.Add(type);
			}
			return result;
		}

		public static bool IsPresentAlongWith<T>(IEnumerable<T> entities, Type compType) where T : Component
		{
			foreach(T ent in entities)
			{
				if(ent.GetComponent(compType)==null)
					return false;
			}
			return true;
		}
	}
}