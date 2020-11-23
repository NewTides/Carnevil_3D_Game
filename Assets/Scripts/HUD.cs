namespace UI
{
	using UnityEngine;
	using UnityEngine.UI;

	/// <summary>
	///  Basic HUD
	/// </summary>
	public class HUD : MonoBehaviour
	{
		/// <summary>
		///  Singleton instance of HUD
		/// </summary>
		public static HUD Instance;

		[SerializeField, Tooltip("The output text UI component")]
		public Text OutputText;

		/// <summary>
		///  True if the output value was recently set
		/// </summary>
		private bool RecentlySet { get; set; }

		/// <summary>
		///  Set the output text
		/// </summary>
		/// <param name="s">The new value of the text field</param>
		public void SetOutputText(string s)
		{
			OutputText.text = s;
			RecentlySet = true;
		}

		// Start is called before the first frame update
		private void Start()
		{
			if (Instance != null)
			{
				DestroyImmediate(gameObject);
			}

			Instance = this;
		}

		// Update is called once per frame
		private void Update()
		{
			if (!RecentlySet)
			{
				OutputText.text = "";
			}

			RecentlySet = false;
		}
	}
}