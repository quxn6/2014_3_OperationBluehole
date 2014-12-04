=====================================================
Blade NPC Chibi Pack [3D Model]  

=====================================================


Update info V1.1    02_23
===================================================

1. It was Applied [Toon Shader -Outline]
 So,  Character is affected by light such as background's light, light prove, or Directional light
 

2. Fixed Animations

-Blade Warrior chibi : [Run],[L_R_Run],[Run01],[L_R_Run01],[Run Attack],[Attack01],[Combo1]

-Blade Girl Chibi : [Run01],[L_R_Run01],[Attack01],[Run Attack],[Block Attack]


3. [BW_Chibi_Base]'s weight value was fixed


-----------------------------------------------------












===================================================

Reperence Script- Swich Equipment

(Thank you to authour on this page ㅠ_ㅠ)

====================================================


   
/********************************************************************
 * Author : Code from masterprompt, project from Berenger
 * 
 * (http://forum.unity3d.com/threads/16485-quot-stitch-multiple-
 * body-parts-into-one-character-quot?p=126864&viewfull=1#post126864)
 * 
 * PS : I did the rig and the animation, but the mesh is from there
 * http://opengameart.org/content/low-poly-base-meshes-male-female
 * ******************************************************************/


using UnityEngine;
using System.Collections;

public class SwitchEquipment : MonoBehaviour 
{
	// 이렇게 생산(?) 할 게임 오브젝트를 받아둔다. 여기선 바지..를 넣자.
	// ※근데 바지가 아니고 팬티같음 -_-
	public GameObject shorty;
	
	// 이렇게 비어있는 게임 오브젝트를 선언 해준다.
	private GameObject shortyOnceWorn;
	
	// 별로 쓸모 없음.
	public GUIText txt; // This is for the demo, can be removed
	
	private bool isWorn = false;
	
	// 그냥 머 키 입력 받는다.
	private void Update()
	{
		if( Input.GetKeyUp( KeyCode.Space ) )
		{
			if( isWorn ) RemoveEquipment();
			else AddEquipment();
		}
	}
	
	// 실험용
//	public Transform testpos;
//	public Material[] testmtrls = new Material[1];
	
	// 착용(?) 해 보자!!
	private void AddEquipment()
	{
		isWorn = true;
		txt.text = "Press space to take it off."; // This is for the demo, can be removed
		
			// Here, boneObj must be instatiated and active (at least the one with the renderer),
			// or else GetComponentsInChildren won't work.
		//// 머 일단 이렇게 아랫쪽에 있을 SkinnedMeshRenderer를 받아둔다.
		SkinnedMeshRenderer[] BonedObjects = shorty.GetComponentsInChildren<SkinnedMeshRenderer>();
		
		// 배열 속에 들어있을 SkinnedMeshRenderer에 대해 뭔가를 해 준다.
		foreach( SkinnedMeshRenderer smr in BonedObjects )
			ProcessBonedObject( smr ); 
		
			// We don't need the old obj, we make it disappear.
		// 머 이렇게 하면 이전에 처리된 오브젝트를 없에준다고 한다. 그냥 랜더 끄는거같은데?
		shorty.SetActiveRecursively( false );
	}
	
	private void RemoveEquipment()
	{
		isWorn = false;		
		txt.text = "Press space to put it on."; // This is for the demo, can be removed
		
		// 없엘 때는 이렇게 없에면 되는 듯 하다.
		Destroy( shortyOnceWorn );
		
		// 다시 착용 안된 바지..를 나타나게 한다.
		shorty.SetActiveRecursively( true );
	}
	
	private void ProcessBonedObject( SkinnedMeshRenderer ThisRenderer )
	{		
			// Create the SubObject
		// 캐릭터에게 입힐 바지 오브젝트를 새로 만들자.
		shortyOnceWorn = new GameObject( ThisRenderer.gameObject.name );
		
		// 이 위치에 새로운 버자(팬티)가 하위 객체로 생성되었다.
	    shortyOnceWorn.transform.parent = transform;
		//shortyOnceWorn.transform.parent = testpos;
	
			// Add the renderer
		// 스크립트 상에서는 랜더러를 이렇게 추가하는듯 하다. 앞에 아무것도 안 쓰면 public인듯.. 근데 함수 끝나면 사라질거 같은데 안사라짐 -_-?
	    SkinnedMeshRenderer NewRenderer = shortyOnceWorn.AddComponent( typeof( SkinnedMeshRenderer ) ) as SkinnedMeshRenderer;
	
			// Assemble Bone Structure
		// 본도 받아야 한다. 일단 크기만큼 할당을 하자. ※왠지 SkinnedMeshRenderer에 본이 있다.
	    Transform[] MyBones = new Transform[ ThisRenderer.bones.Length ];
	
			// As clips are using bones by their names, we find them that way.
		// 이 함수는 아래에 있다.
	    for( int i = 0; i < ThisRenderer.bones.Length; i++ )
	        MyBones[ i ] = FindChildByName( ThisRenderer.bones[ i ].name, transform ); // 하위 본을 전부 맞추는게 편하니 이렇게 하자.
		// 아니면 각 원소간 하나 하나 수동으로 넣어도 될 법 하다.
	
	    	// Assemble Renderer
		// 랜더러를 할당한다..고 한다. 바지(팬티)의 새로운 랜더로써, 갖가지 기능을 여기서 처리할 수 있다. matrial을 바꿔서 바지 색을 바꾼다던가..(누런 팬티)
	    NewRenderer.bones = MyBones;
	    NewRenderer.sharedMesh = ThisRenderer.sharedMesh; // 어쨰서 그냥 mesh라는 키워드가 없는지는 모르겠는데 그게 이건가보다.
	    NewRenderer.materials = ThisRenderer.materials;
		//NewRenderer.material = testmtrls[0];
	}
	
		// Recursive search of the child by name.
	// 머 뺑뺑이 돌리면서 찾는다고 한다.
	private Transform FindChildByName( string ThisName, Transform ThisGObj )	
	{	
		// 리턴용 임시 Transform.
	    Transform ReturnObj;
	
			// If the name match, we're return it
		// 검색 조건에 맞으면 리턴한다.
	    if( ThisGObj.name == ThisName )	
	        return ThisGObj.transform;
	
			// Else, we go continue the search horizontaly and verticaly
		// 안 맞으면 계층구조의 가로 세로로 계속 찾게 한다.
		// 아무래도, child의 child의... 해서 찾게 하려고 이렇게 하는 듯.
		
		// 대체 foreach문이 먼진 모르겠는데, 대충 문맥상 in 오른쪽에 들어있는 왼쪽 타입에 대해...라는 구문 같다.
	    foreach( Transform child in ThisGObj )	
	    {	
			// 재귀함수?!
	        ReturnObj = FindChildByName( ThisName, child );
	
	        if( ReturnObj != null )	
	            return ReturnObj;	
	    }
	
	    return null;	
	}	
}


==================================================================







asset info

-----------------------------------------------------
models
-----------------------------------------------------

> This Character are divided into 4 parts
[Hair, Face, Body, Weapone]





===========================================================================
Chibi Detail info
 ===========================================================================
 
Information in 3D max (It is increased in Unity 

[ 3Dmax vert + UV Seem vert + Vertex according to the Material and light =Unity verts] 

----------------------------------------------------------------------------------------
 
Blade girl         Base       01         02          03      04        05       06           07        average                                         
Vertex              445       438        432        490      562      653       640         814          559
Tris                675       668        668        766      878      994       999         1329         872
 
---------------------------------------------------------------------------------------------------------
 
Blade warrior     Base     01         02        03         04         05          06         07            average
Vertex             436     417        375      487        539         704         681       809              556
Tris               658     644        610      723        825         1044        1017     1268              849
 
---------------------------------------------------------------------------------------------------------
 
 



=================================================== 

instructions

===================================================


1.click to the  [File] > [Open Project] > [Demo_scene] folder

2.Dubble click [Blade warrior or Blade girl 's Demo_scene]

3.you can use the model file ~  Too easy~~~









Enjoy!! Cool Guys!!
================================================= 

If you have a question or comment

Send to my E-mail


kimys2848@naver.com

  

Visit my site for more info


>> http://blog.naver.com/kimys2848





