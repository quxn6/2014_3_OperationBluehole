using UnityEngine;
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
		NetworkManager.Instance.SignupRequest(
			SignupIDForm.GetComponent<UILabel>().text ,
			SignupPWForm.GetComponent<UILabel>().text ,
			SignupNameForm.GetComponent<UILabel>().text );
		//Debug.Log( "submit signup form" );
	}

	// post login data
	public void SubmitLoginForm()
	{
		NetworkManager.Instance.LoginRequest(
			LoginIDForm.GetComponent<UILabel>().text ,
			LoginPWForm.GetComponent<UILabel>().text
			);
		//Debug.Log( "submit login form" );
	}
}
