using UnityEngine;
using System.Collections;

public class TitleMenu : MonoBehaviour
{
	public GameObject loginButton = null;
	public GameObject signUpButton = null;
	public GameObject playButton = null;

	public GameObject loginForm = null;
	public GameObject signUpForm = null;

	void Start()
	{
		// check authentication
	}


	public void LoadloginForm()
	{
		loginForm.SetActive( true );
	}

	public void LoadSignUpForm()
	{
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
		Debug.Log( "submit signup form" );
	}

	// post login data
	public void SubmitLoginForm()
	{
		Debug.Log( "submit login form" );
	}
}
