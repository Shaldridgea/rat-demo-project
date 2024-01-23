using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RAT
{
	/// <summary>
	/// Self-destructing reference class to load RAT resources on demand
	/// </summary>
	public class ReferenceRAT : MonoBehaviour
	{
		public List<RAT_Settings> references;

		private void Start()
		{
			RemoveNullReferences();
			Destroy(gameObject);
		}

		public void RemoveNullReferences()
		{
			for (int i = references.Count - 1; i >= 0; --i)
				if (references[i] == null)
					references.RemoveAt(i);
		}
	}
}