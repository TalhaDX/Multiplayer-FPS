﻿using UnityEngine;
using System.Collections;
using UnityStandardAssets.CrossPlatformInput;
using UnityStandardAssets.Characters.FirstPerson;

public class PlayerNetworkMover : Photon.MonoBehaviour {

	public Animator anim;

	Vector3 position;
	Quaternion rotation;
	float horizontal;
	float vertical;
	bool jump;
	float smoothing = 10.0f;
	float dampTime = 0.06f;

	void MoveToLayer(Transform root, int layer) {
		root.gameObject.layer = layer;
		foreach(Transform child in root)
			MoveToLayer(child, layer);
	}

	void Start() {
		if (photonView.isMine) {
			GetComponent<CharacterController>().enabled = true;
			GetComponent<FirstPersonController>().enabled = true;
			GetComponentInChildren<AudioListener>().enabled = true;
			GetComponentInChildren<GunFirstPersonView>().enabled = true;
			foreach (Camera camera in GetComponentsInChildren<Camera>()) {
				camera.enabled = true;
			}
			MoveToLayer(this.transform.FindChild("T_Ak-47"), LayerMask.NameToLayer("Hidden"));
			MoveToLayer(this.transform.FindChild("FPSMainCamera/F_Ak-47"), LayerMask.NameToLayer("FPSGun"));
			MoveToLayer(this.transform.FindChild("PlayerModel"), LayerMask.NameToLayer("Hidden"));
		} else {
			StartCoroutine(UpdateData());
		}
	}

	IEnumerator UpdateData() {
		while (true) {
			transform.position = Vector3.Lerp(transform.position, position, Time.deltaTime * smoothing);
			transform.rotation = Quaternion.Lerp(transform.rotation, rotation, Time.deltaTime * smoothing);
			yield return null;
		}
	}

	void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {
		if (stream.isWriting) {
			stream.SendNext(transform.position);
			stream.SendNext(transform.rotation);
			stream.SendNext(CrossPlatformInputManager.GetAxis("Horizontal"));
			stream.SendNext(CrossPlatformInputManager.GetAxis("Vertical"));
			stream.SendNext(CrossPlatformInputManager.GetButtonDown("Jump"));
			stream.SendNext(anim.GetBool("Running"));
		} else {
			position = (Vector3)stream.ReceiveNext();
			rotation = (Quaternion)stream.ReceiveNext();
			horizontal = (float)stream.ReceiveNext();
			vertical = (float)stream.ReceiveNext();
			anim.SetFloat("Horizontal", horizontal, dampTime, Time.deltaTime);
			anim.SetFloat("Vertical", vertical, dampTime, Time.deltaTime);
			if ((bool)stream.ReceiveNext()) 
				anim.SetTrigger("IsJumping");
			anim.SetBool("Running", (bool)stream.ReceiveNext());
		}
	}
}
