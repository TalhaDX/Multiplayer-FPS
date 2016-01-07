﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class NetworkManager : MonoBehaviour {
	
	[SerializeField] Text connectionText;
	[SerializeField] Transform[] spawnPoints;
	[SerializeField] Camera sceneCamera;

	GameObject player;

	void Start() {
		PhotonNetwork.logLevel = PhotonLogLevel.Full;
		PhotonNetwork.ConnectUsingSettings("0.1");
	}

	void Update() {
		connectionText.text = PhotonNetwork.connectionStateDetailed.ToString();
	}

	void OnJoinedLobby() {
		RoomOptions room = new RoomOptions() {isVisible = true, maxPlayers = 12};
		PhotonNetwork.JoinOrCreateRoom("Armour", room, TypedLobby.Default);
	}

	void OnJoinedRoom() {
		StartSpawnProcess(0.0f);
	}

	void StartSpawnProcess(float spawnTime) {
		sceneCamera.enabled = true;
		StartCoroutine(SpawnPlayer(spawnTime));
	}

	IEnumerator SpawnPlayer(float spawnTime) {
		yield return new WaitForSeconds(spawnTime);

		int index = Random.Range (0, spawnPoints.Length);
		player = PhotonNetwork.Instantiate("PolicemanController", spawnPoints[index].position, spawnPoints[index].rotation, 0);

		player.GetComponent<PlayerHealth>().RespawnMe += StartSpawnProcess;

		sceneCamera.enabled = false;
	}
}
