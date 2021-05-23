using System;
using System.Collections;
using System.Collections.Generic;
using Unify.UnifiedRenderer;
using UnityEngine;
using Random = UnityEngine.Random;

public class DemoBall : MonoBehaviour {

	[SerializeField]
	private float throwStrength = 30f;
	
	private Rigidbody _rigid;
	private UnifiedRenderer _unifiedRenderer;

	private float _currentHue;
	
	private void Start() {
		_rigid           = GetComponent<Rigidbody>();
		_unifiedRenderer = GetComponent<UnifiedRenderer>();

		_currentHue = Random.Range(0f, 10f);
	}

	private void Update() {
		_currentHue += Time.deltaTime * 0.1f;
		
		_unifiedRenderer.SetMaterialProperty("_BaseColor", Color.HSVToRGB(_currentHue % 1, 1, 1));
		_unifiedRenderer.SetMaterialProperty("_EmissionColor", Color.HSVToRGB(_currentHue % 1, 1, 1));
	}

	private void OnTriggerEnter(Collider other) {
		if (other.CompareTag("Pit")) {
			var randDir = Random.onUnitSphere;
			randDir   *= 0.2f;

			randDir.y = 1;
			randDir.Normalize();
			
			_rigid.AddForce(randDir * throwStrength, ForceMode.VelocityChange);
		}
	}
}