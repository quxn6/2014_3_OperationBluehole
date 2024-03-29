﻿using UnityEngine;
using System.Collections;

public class TitleMenu : MonoBehaviour
{
	public GameObject loginButton = null;
	public GameObject signUpButton = null;
	public GameObject playButton = null;

	public GameObject LoginIDForm = null;
	public GameObject LoginPWForm = null;
	public GameObject SignupIDForm = null;
	public GameObject SignupPWForm = null;
	public GameObject SignupNameForm = null;

	public GameObject loginForm = null;
	public GameObject signUpForm = null;

	void Start()
	{
		// check authentication
	}


	public void LoadloginForm()
	{
		CloseForm();
		loginForm.SetActive( true );
	}

	public void LoadSignUpForm()
	{
		CloseForm();
		signUpForm.SetActive( true );
	}

	public void LoadPlayScene()
	{
		Application.LoadLevel( "mainMenu" );
	}

	// close all forms
	public void CloseForm()
	{
		loginForm.SetActive( false );
		signUpForm.SetActive( false );
	}
	
	// post signup data
	public void SubmitSignUpForm()
	{
		StartCoroutine( SignupProcess() );
		//Debug.Log( "submit signup form" );
	}

	private IEnumerator SignupProcess()
	{
		string id = SignupIDForm.GetComponent<UILabel>().text;		
		string pw = SignupPWForm.GetComponent<UILabel>().text;
		string name = SignupNameForm.GetComponent<UILabel>().text;

		yield return StartCoroutine( NetworkManager.Instance.SignupRequest( id , pw , name ) );		
		CloseForm();
		NetworkManager.Instance.LoginRequest( id , pw );
	}

	// post login data
	public void SubmitLoginForm()
	{
		NetworkManager.Instance.LoginRequest(
			LoginIDForm.GetComponent<UILabel>().text ,
			LoginPWForm.GetComponent<UILabel>().text
			);
		CloseForm();
		//Debug.Log( "submit login form" );
	}
}
