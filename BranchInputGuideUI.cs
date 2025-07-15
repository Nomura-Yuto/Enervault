using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using Unity.VisualScripting;

public class BranchInputGuideUI : MonoBehaviour
{

	[System.Serializable]
	public class BranchInputGuide
	{
		public Sprite all;// 全方向に進行可能な場合の画像
		public Sprite up;// 前方に進行可能な場合の画像
		public Sprite left;// 左に進行可能な場合の画像
		public Sprite right;// 右に進行可能な場合の画像
		public Sprite upLeft;// 左上に進行可能な場合の画像
		public Sprite upRight;// 右上に進行可能な場合の画像
		public Sprite leftRight;// 左右に進行可能な場合の画像
		public Sprite confirm;// 確定ボタンの画像
	}

	[Header("操作ガイド設定")]
	[Tooltip("操作ガイドUI")]
	[SerializeField] Image inputGuideImage;
	[SerializeField] Image ofImage; // 画像の参照を保持するためのフィールド
	[SerializeField] Image confirmImage; // 確定ボタンの画像を保持するためのフィールド

	[SerializeField] Sprite ofImageSprite;



	[SerializeField] PlayerController playerController;
	[SerializeField] PlayerInput playerInput;
	



	[Tooltip("キーボードのガイド画像")]
	[SerializeField] BranchInputGuide keyboardGuide;
	[Tooltip("ゲームパッドのガイド画像")]
	[SerializeField] BranchInputGuide gamepadGuide;
	AdjacentTile _underTile;

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
	{
		inputGuideImage.gameObject.SetActive(false);
		confirmImage.gameObject.SetActive(false);
		if (ofImage != null)
		{
			ofImage.sprite = ofImageSprite;
			ofImage.gameObject.SetActive(false);
		}
	}


	/// <summary>進行可能な方向に応じた操作ガイドの表示</summary>
	public void DispInputGuide()
	{

		if (playerController == null || playerInput == null || inputGuideImage == null)
		{
			Debug.LogWarning("BranchInputGuideUI: PlayerController, PlayerInput, or InputGuide is not set.");
			return;
		}


		_underTile = playerController.tileActor.GetAdjacentBottomTiles();

		Sprite dispSprite;
		BranchInputGuide guideImages;
		string controlDevice = playerInput.currentControlScheme;

		bool useFront = (_underTile.front != null);
		bool useLeft = (_underTile.left != null);
		bool useRight = (_underTile.right != null);

		// 現在のデバイスに応じた画像を指定
		switch (controlDevice)
		{
			case "Keyboard&Mouse":
				guideImages = keyboardGuide;
				break;

			case "Gamepad":
				guideImages = gamepadGuide;
				break;

			default:
				return;
		}

		// 進行できる方向に応じて画像を変更
		if (useFront && !useLeft && !useRight)
		{
			dispSprite = guideImages.up;
		}
		else if (!useFront && useLeft && !useRight)
		{
			dispSprite = guideImages.left;
		}
		else if (!useFront && !useLeft && useRight)
		{
			dispSprite = guideImages.right;
		}
		else if (!useFront && useLeft && useRight)
		{
			dispSprite = guideImages.leftRight;
		}
		else if (useFront && !useLeft && useRight)
		{
			dispSprite = guideImages.upRight;
		}
		else if (useFront && useLeft && !useRight)
		{
			dispSprite = guideImages.upLeft;
		}
		else
		{
			dispSprite = guideImages.all;
		}

		// 分岐タイルなら確定ボタンを表示
		bool isWarp = playerController.tileActor.GetUnderTileInfo().tileObject.CompareTag("Warp");
		if (isWarp)
		{
			confirmImage.sprite = guideImages.confirm;
			confirmImage.gameObject.SetActive(true);
			ofImage?.gameObject.SetActive(true);
		}
		else
		{
			confirmImage.gameObject.SetActive(false);
			ofImage?.gameObject.SetActive(false);
		}

		// 入力ガイド（方向）を表示
		inputGuideImage.sprite = dispSprite;

		inputGuideImage.gameObject.SetActive(true);
	}

	public void CloseInputGuide()
	{
		inputGuideImage.gameObject.SetActive(false);
		confirmImage.gameObject.SetActive(false);
		if (ofImage != null)
		{
			ofImage.gameObject.SetActive(false);
		}
	}
}
