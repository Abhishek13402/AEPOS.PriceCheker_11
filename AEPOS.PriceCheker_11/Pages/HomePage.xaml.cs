using AEPOS.DAO;
using AEPOS.Model;
using AEPOS.PriceCheker_11;
using Microsoft.Maui.Controls.PlatformConfiguration;
using Sypram.Common;
using System.Collections.ObjectModel;
using System.Data;
using System.Globalization;
using System.Security.Cryptography.X509Certificates;
using Telerik.Windows.Documents.Spreadsheet.Expressions.Functions;

namespace AEPOS.PriceChecker_11.Pages;

public partial class HomePage : ContentPage
{
	public ObservableCollection<SearchFilter> Items { get; set; } = new ObservableCollection<SearchFilter>();
	NetworkAccess accessType = Connectivity.Current.NetworkAccess;
	private System.Timers.Timer _eventTimer;
	private const int TimerDuration = 20000; // 20 seconds in milliseconds
	private bool bIsScrollBtnClicked = false;
	public HomePage()
	{
		CheckInternetAndExit();
		InitializeComponent();
		InitializeTimer();
		Loaded += MainPage_Loaded;
		SearchEntry.Completed += OnBarcodeScanned;
		storename.Text = Globals.StoreName;
		listSection.IsVisible = false;
		detailSection.IsVisible = false;
		SearchEntry.Focus();
		introSection.IsVisible = true;
	}
	bool topleft_btn = false;
	bool topright_btn = false;
	bool bottomright_btn = false;
	bool bottomleft_btn = false;
	bool isCapsLock_Enable = false;
	bool isShift_Pressed = false;
	string cutText;
	public bool isbarcodescan = false;
	private void MainPage_Loaded(object sender, EventArgs e)
	{
		try
		{
			SearchEntry.Focus();
		}
		catch (Exception ex)
		{
			DisplayAlert("Error", $"An error occurred: {ex.Message}", "OK");
			return;
		}
	}
	private void InitializeTimer()
	{
		try
		{
			_eventTimer = new System.Timers.Timer(TimerDuration);
			_eventTimer.Elapsed += OnTimerElapsed;
			_eventTimer.AutoReset = false; // Timer runs once per start
		}
		catch (Exception ex)
		{
			DisplayAlert("Error", $"An error occurred: {ex.Message}", "OK");
			return;
		}
	}
	private void OnTimerElapsed(object sender, System.Timers.ElapsedEventArgs e)
	{
		try
		{
			Dispatcher.Dispatch(() =>
			{
				lblneedprice.Text = "Need a Price ??";
				lblsimplytext.IsVisible = true;
				scrollToBottomButton.IsVisible = false;
				scrollToTopButton.IsVisible = false;

				detailSection.IsVisible = false;
				if (listSection.IsVisible == true)
				{
					introSection.IsVisible = false;
				}

				if (KeyboardLayout.IsVisible == true)
				{
					introSection.IsVisible = false;
				}
				else
				{
					introSection.IsVisible = true;
				}
				SearchEntry.Focus();
			});
		}
		catch (Exception ex)
		{
			DisplayAlert("Error", $"An error occurred: {ex.Message}", "OK");
			return;
		}
	}
	public async void CheckInternetAndExit()
	{
		try
		{
			if (Connectivity.NetworkAccess != NetworkAccess.Internet)
			{
				bool status = await DisplayAlert("Internet is Required!!", "Please Check your Internet", "Exit", "cancel");
				if (status)
				{
					CloseApp();
				}
				return;
			}
			//DisplayAlert("Internet Status", "You are connected to the Internet.", "OK");
		}
		catch (Exception ex)
		{
			DisplayAlert("Error", $"An error occurred: {ex.Message}", "OK");
			return;
		}
	}
	//private async void btnSearch_Clicked(string UPC)
	//{
	//    try
	//    {
	//        listSection.IsVisible = false;
	//        detailSection.IsVisible = true;
	//        SearchEntry.Text = "";
	//        Items.Clear();
	//        // loadingpopup.Hide();

	//        if (string.IsNullOrEmpty(UPC))
	//        {
	//            await DisplayAlert("Alert !", "Enter UPC", "ok");
	//            return;
	//        }
	//        if (Globals.Isnet_Connected == true)
	//        {
	//            if (Globals.ConnectToDatabase(true) == false)
	//            {
	//                Globals.IsServerConnected = false;
	//            }
	//            ITEMINFO_DAL Iteminfo_DAL = new ITEMINFO_DAL(Globals.DBServer);

	//            DataTable ItemData = Iteminfo_DAL.GetItemList(UPC);

	//            if (ItemData != null && ItemData.Rows.Count > 0)
	//            {
	//                //lblSKU.Text = ItemData.Rows[0]["SKU"].ToString() != null ? ItemData.Rows[0]["SKU"].ToString() : "";
	//                lblName.Text = ItemData.Rows[0]["ITEMNAME"].ToString() != null ? ItemData.Rows[0]["ITEMNAME"].ToString() : "";
	//                //lblUPC.Text = ItemData.Rows[0]["MAINUPC"].ToString() != null ? ItemData.Rows[0]["MAINUPC"].ToString() : "";
	//            }
	//            else
	//            {
	//                detailSection.IsVisible = false;
	//                introSection.IsVisible = true;
	//                //DisplayAlert("Item not Found!", "Item not found. Invalid QR is Scanned !", "ok");
	//                lblneedprice.Text = "Item not Found!";
	//                return;
	//            }
	//        }
	//        else
	//        {
	//            detailSection.IsVisible = false;
	//            introSection.IsVisible = true;
	//            //DisplayAlert("Item not Found!", "Item not found. Invalid QR is Scanned !", "ok");
	//            lblneedprice.Text = "Item not Found!";
	//            return;
	//        }
	//    }
	//    catch (Exception ex)
	//    {
	//        DisplayAlert("Error", $"An error occurred: {ex.Message}", "OK");
	//    }
	//}
	private void SearchItemName(object sender, EventArgs e)
	{
		try
		{
			CheckInternetAndExit();
			string SKU = SearchEntry.Text;
			Items.Clear();
			if (SKU.Length >= 3)
			{
				listSection.IsVisible = true;
				detailSection.IsVisible = false;
				if (Globals.Isnet_Connected == true)
				{
					if (Globals.ConnectToDatabase(true) == false)
					{
						Globals.IsServerConnected = false;
						SearchEntry.Text = "";
						//DisplayAlert("No Internet Connection!", "Unable to connect to database.", "ok");
						SearchEntry.Focus();
						return;
					}
					SearchByKeyword("", SKU);
					//ITEMINFO_DAL Iteminfo_DAL = new ITEMINFO_DAL(Globals.DBServer);
					//DataTable ItemData = Iteminfo_DAL.GetItemListByName(SKU);

					//if (ItemData != null && ItemData.Rows.Count > 0)
					//{
					//	lblName.Text = ItemData.Rows[0]["ITEMNAME"].ToString();
					//	lblSKU.Text = ItemData.Rows[0]["SKU"].ToString();
					//	//lblUPC.Text = ItemData.Rows[0]["MAINUPC"].ToString();
					//	foreach (DataRow row in ItemData.Rows)
					//	{
					//		Items.Add(new Item
					//		{
					//			SKU = row["SKU"].ToString(),
					//			ITEMNAME = row["ITEMNAME"].ToString(),
					//			//SizeName = row["SizeName"].ToString(),
					//			//PackName = row["PackName"].ToString(),
					//			//DepName = row["DepName"].ToString(),
					//			//CatName = row["CatName"].ToString(),
					//			//RegPrice = System.Convert.ToDecimal(row["RegPrice"].ToString()),
					//			//SalePrice = System.Convert.ToDecimal(row["SalePrice"].ToString()),
					//			//Location = row["Location"].ToString(),
					//			//Description = row["Description"].ToString(),
					//			//CurrencySymbol = row["CurrencySymbol"].ToString(),
					//			//strUPC = row["strUPC"].ToString(),
					//		});
					//	}
					//	MyCollectionView.ItemsSource = Items;
					//}
					//else
					//{
					//	//DisplayAlert("Item not Found !", "Item not found. Invalid Barcode  is Scanned !", "ok");
					//	return;
					//}
				}
				else
				{
					return;
				}
			}
			SearchEntry.Focus();
		}
		catch (Exception ex)
		{
			DisplayAlert("Error", $"An error occurred: {ex.Message}", "OK");
			return;
		}
	}
	public class Item
	{
		public string SKU { get; set; }
		public string ITEMNAME { get; set; }
		public string SizeName { get; set; }
		public string PackName { get; set; }
		public string DepName { get; set; }
		public string CatName { get; set; }
		public decimal RegPrice { get; set; }
		public decimal SalePrice { get; set; }
		public string Location { get; set; }
		public string Description { get; set; }
		public string CurrencySymbol { get; set; }
		public string strUPC { get; set; }
	}
	private async void OnBarcodeScanned(object sender, EventArgs e)
	{
		try
		{
			loadingpopup.Show();
			if (KeyboardLayout.IsVisible == true)
			{
				KeyboardLayout.IsVisible = false;
			}
			CheckInternetAndExit();
			introSection.IsVisible = false;
			string scannedData = SearchEntry.Text;
			isbarcodescan = true;
			bool ScannedSuccessfully = await SearchItemBySKU(scannedData);
			if (ScannedSuccessfully == true)
			{
				SizeEvaluation();
			}
			//btnSearch_Clicked(scannedData);
			SearchEntry.Text = string.Empty;
			//StartTimer();
			ResetTimer();
		}
		catch (Exception ex)
		{
			DisplayAlert("Error", $"An error occurred: {ex.Message}", "OK");
			return;
		}
	}
	private async void OnItemTapped(object sender, TappedEventArgs e)
	{
		try
		{
			if (e.Parameter is int selectedSKU)
			{
				loadingpopup.Show();
				introSection.IsVisible = false;

				bool ItmFound = await SearchItemBySKU(selectedSKU.ToString());
				if (ItmFound == true)
				{
					SizeEvaluation();
				}

				//btnSearch_Clicked(selectedUPC);
				KeyboardLayout.IsVisible = false;
			//StartTimer();
				ResetTimer();
				//ScrollToBottomButton_Clicked(sender, e);
				//await detailSection.ScrollToAsync(0, detailSection.ContentSize.Height, true);

			}
		}
		catch (Exception ex)
		{
			await DisplayAlert("Error", $"An error occurred: {ex.Message}", "OK");
			return;
		}
	}
	private void StartTimer()
	{
		try
		{
			 _eventTimer.Start();
		}
		catch (Exception ex)
		{
			DisplayAlert("Error", $"Failed to start timer: {ex.Message}", "OK");
			return;
		}
	}
	private  void ResetTimer()
	{
		try
		{
			StopTimer();
			StartTimer();
		}
		catch (Exception ex)
		{
			DisplayAlert("Error", $"An error occurred: {ex.Message}", "OK");
			return;
		}
	}
	private  void StopTimer()
	{
		try
		{
			_eventTimer.Stop();
		}
		catch (Exception ex)
		{
			DisplayAlert("Error", $"An error occurred: {ex.Message}", "OK");
			return;
		}
	}
	private void RefreshButton_Clicked(object sender, EventArgs e)
	{
		try
		{
			CheckInternetAndExit();
			detailSection.IsVisible = false;
			SearchEntry.Text = "";
			SearchEntry.Focus();
			lblneedprice.Text = "Need a Price ??";
			lblsimplytext.IsVisible = true;

			scrollToBottomButton.IsVisible = false;
			scrollToTopButton.IsVisible = false;

			topleft_btn = false;
			topright_btn = false;
			bottomright_btn = false;
			bottomleft_btn = false;
			if (KeyboardLayout.IsVisible == true)
			{
				introSection.IsVisible = false;
			}
			else
			{
				introSection.IsVisible = true;
			}
			ResetTimer();
		}
		catch (Exception ex)
		{
			DisplayAlert("Error", $"An error occurred: {ex.Message}", "OK");
			return;
		}
	}

	private void CloseApp()
	{
		try
		{
#if ANDROID
			Android.OS.Process.KillProcess(Android.OS.Process.MyPid());
#endif
		}
		catch (Exception ex)
		{
			DisplayAlert("Error", $"An error occurred: {ex.Message}", "OK");
			return;
		}
	}

	#region Keyboard methods
	//keyborad methods-----------------------------------------------------
	private void keyboardbtn_clicked(object sender, EventArgs e)
	{
		try
		{
			if (KeyboardLayout.IsVisible == false)
			{
				KeyboardLayout.IsVisible = true;
			}
			if (SearchEntry.Text != null)
			{
				SearchEntry.Text = "";
				SearchEntry.Focus();
				listSection.IsVisible = false;
			}
			introSection.IsVisible = false;
			KeyboardLayout.IsVisible = true;
			RefreshButton_Clicked(sender, e);
		}
		catch (Exception ex)
		{
			DisplayAlert("Error", $"An error occurred: {ex.Message}", "OK");
			return;
		}
	}
	private void HomeBtnCloseKeyboard(object sender, EventArgs e)
	{
		try
		{
			KeyboardLayout.IsVisible = false;
			SearchEntry.Focus();
			RefreshButton_Clicked(sender, e);
		}
		catch (Exception ex)
		{
			DisplayAlert("Error", $"An error occurred: {ex.Message}", "OK");
			return;
		}
	}
	private void Backspace_Tap(object sender, EventArgs e)
	{
		try
		{
			if (SearchEntry.Text != null)
			{
				if (SearchEntry.Text.Length > 0)
				{
					int cursorPosition = SearchEntry.CursorPosition;
					if (cursorPosition > 0)
					{
						string originalText = SearchEntry.Text ?? string.Empty;
						string newText = originalText.Remove(cursorPosition - 1, 1);
						SearchEntry.Text = newText;
						SearchEntry.CursorPosition = cursorPosition - 1;
					}
					SearchEntry.Focus();
				}
			}
		}
		catch (Exception ex)
		{
			DisplayAlert("Alert !", $"{ex.Message}", "ok");
			return;
		}
	}
	private void CapsLock_Tap(object sender, EventArgs e)
	{
		try
		{
			if (isCapsLock_Enable == true)
			{
				btnCaps.BorderColor = Colors.Black;
				btnCaps.BorderWidth = 0;
				isCapsLock_Enable = false;
				LetA.Text = "a";
				LetB.Text = "b";
				LetC.Text = "c";
				LetD.Text = "d";
				LetE.Text = "e";
				LetF.Text = "f";
				LetG.Text = "g";
				LetH.Text = "h";
				LetI.Text = "i";
				LetJ.Text = "j";
				LetK.Text = "k";
				LetL.Text = "l";
				LetM.Text = "m";
				LetN.Text = "n";
				LetO.Text = "o";
				LetP.Text = "p";
				LetQ.Text = "q";
				LetR.Text = "r";
				LetS.Text = "s";
				LetT.Text = "t";
				LetU.Text = "u";
				LetV.Text = "v";
				LetW.Text = "w";
				LetX.Text = "x";
				LetY.Text = "y";
				LetZ.Text = "z";
			}
			else
			{
				btnCaps.BorderColor = Colors.Black;
				btnCaps.BorderWidth = 2;
				isCapsLock_Enable = true;
				LetA.Text = "A";
				LetB.Text = "B";
				LetC.Text = "C";
				LetD.Text = "D";
				LetE.Text = "E";
				LetF.Text = "F";
				LetG.Text = "G";
				LetH.Text = "H";
				LetI.Text = "I";
				LetJ.Text = "J";
				LetK.Text = "K";
				LetL.Text = "L";
				LetM.Text = "M";
				LetN.Text = "N";
				LetO.Text = "O";
				LetP.Text = "P";
				LetQ.Text = "Q";
				LetR.Text = "R";
				LetS.Text = "S";
				LetT.Text = "T";
				LetU.Text = "U";
				LetV.Text = "V";
				LetW.Text = "W";
				LetX.Text = "X";
				LetY.Text = "Y";
				LetZ.Text = "Z";
			}
		}
		catch (Exception ex)
		{
			DisplayAlert("Error", $"An error occurred: {ex.Message}", "OK");
			return;
		}
	}
	private void OnKeyClicked(object sender, EventArgs e)
	{
		try
		{
			if (sender is Button button)
			{
				string KeyText;
				if (isCapsLock_Enable == true)
				{
					KeyText = button.Text.ToUpper();
				}
				else
				{
					KeyText = button.Text.ToLower();
				}
				SearchEntry.Focus();
				InsertTextAtCursor(KeyText);
			}
		}
		catch (Exception ex)
		{
			DisplayAlert("Error !", $"{ex.Message}", "ok");
			return;
		}
	}
	private void InsertTextAtCursor(string text)
	{
		try
		{
			int cursorPosition = SearchEntry.CursorPosition;
			string currentText = SearchEntry.Text;
			if (currentText == null)
			{
				currentText = string.Empty;
			}
			string newText = currentText.Insert(cursorPosition, text);
			SearchEntry.Text = newText;
			SearchEntry.CursorPosition = cursorPosition + text.Length;
		}
		catch (Exception ex)
		{
			DisplayAlert("Alert !", $"{ex.Message}", "ok");
			return;
		}
	}
	private void OnKeyClear(object sender, EventArgs e)
	{
		try
		{
			SearchEntry.Text = "";
		}
		catch (Exception ex)
		{
			DisplayAlert("Error", $"An error occurred: {ex.Message}", "OK");
			return;
		}
	}
	private void OnKeyCut(object sender, EventArgs e)
	{
		try
		{
			cutText = SearchEntry.Text;
			SearchEntry.Text = "";
		}
		catch (Exception ex)
		{
			DisplayAlert("Error", $"An error occurred: {ex.Message}", "OK");
			return;
		}
	}
	private void OnKeyPaste(object sender, EventArgs e)
	{
		try
		{
			InsertTextAtCursor(cutText);
		}
		catch (Exception ex)
		{
			DisplayAlert("Error", $"An error occurred: {ex.Message}", "OK");
			return;
		}
	}
	private void OnKeyShift(object sender, EventArgs e)
	{
		try
		{
			if (isShift_Pressed == true)
			{
				btnShift.BorderColor = Colors.Black;
				btnShift.BorderWidth = 0;
				isShift_Pressed = false;
				num1.Text = "1";
				num2.Text = "2";
				num3.Text = "3";
				num4.Text = "4";
				num5.Text = "5";
				num6.Text = "6";
				num7.Text = "7";
				num8.Text = "8";
				num9.Text = "9";
				num0.Text = "0";
			}
			else
			{
				btnShift.BorderColor = Colors.Black;
				btnShift.BorderWidth = 2;
				isShift_Pressed = true;
				num1.Text = "!";
				num2.Text = "@";
				num3.Text = "#";
				num4.Text = "$";
				num5.Text = "%";
				num6.Text = "^";
				num7.Text = "&";
				num8.Text = "*";
				num9.Text = "-";
				num0.Text = "/";
			}
		}
		catch (Exception ex)
		{
			DisplayAlert("Error", $"An error occurred: {ex.Message}", "OK");
			return;
		}
	}
	private void InputChange(object sender, EventArgs e)
	{
		try
		{
			DisplayAlert("", "", "Ok");
		}
		catch (Exception ex)
		{
			DisplayAlert("Error", $"An error occurred: {ex.Message}", "OK");
			return;
		}
	}
	private async void Enter_Button_Clicked(object sender, EventArgs e)
	{
		try
		{
			SearchFilter firstitem = Items.FirstOrDefault();
			if (SearchEntry.Text == "")
			{
				DisplayAlert("Alert!!", "Please Enter Value For Search", "OK");
				return;
			}
			if (firstitem != null)
			{
				//btnSearch_Clicked(firstitem.ID);
				bool ItmFound = await SearchItemBySKU(firstitem.ID.ToString());
				if (ItmFound == true)
				{
					SizeEvaluation();
				}
			}
			else
			{
				RefreshButton_Clicked(sender, e);
				detailSection.IsVisible = false;
				introSection.IsVisible = true;
				lblneedprice.Text = "Item not Found!";
				lblsimplytext.IsVisible = false;
				if (KeyboardLayout.IsVisible == true)
				{
					KeyboardLayout.IsVisible = false;
					introSection.IsVisible = true;
				}
				//DisplayAlert("Alert!!", "Item Not Found", "OK");
				ResetTimer();
			}
		}
		catch (Exception ex)
		{
			DisplayAlert("Error", $"An error occurred: {ex.Message}", "OK");
			return;
		}
	}
	#endregion

	#region logout 
	private async void LogOut()
	{
		try
		{
			if (topleft_btn.Equals(true) && topright_btn.Equals(true) && bottomleft_btn.Equals(true) && bottomright_btn.Equals(true))
			{
				await App.cacheDataRepo.RemoveStoreData();
				Globals.StoreName = "";
				Globals.Store_ID = "";
				storename.Text = "";
				Globals.Server_ConnStr = "";
				Navigation.PopAsync();
			}
		}
		catch (Exception ex)
		{
			DisplayAlert("Error", $"An error occurred: {ex.Message}", "OK");
			return;
		}
	}
	private void topleftbtn_click(object sender, EventArgs e)
	{
		try
		{
			topleft_btn = true;
			LogOut();
		}
		catch (Exception ex)
		{
			DisplayAlert("Error", $"An error occurred: {ex.Message}", "OK");
			return;
		}
	}
	private void toprightbtn_click(object sender, EventArgs e)
	{
		try
		{
			topright_btn = true;
			LogOut();
		}
		catch (Exception ex)
		{
			DisplayAlert("Error", $"An error occurred: {ex.Message}", "OK");
			return;
		}
	}
	private void bottomleftbtn_click(object sender, EventArgs e)
	{
		try
		{
			bottomleft_btn = true;
			LogOut();
		}
		catch (Exception ex)
		{
			DisplayAlert("Error", $"An error occurred: {ex.Message}", "OK");
			return;
		}
	}
	private void bottomrightbtn_click(object sender, EventArgs e)
	{
		try
		{
			bottomright_btn = true;
			LogOut();
		}
		catch (Exception ex)
		{
			DisplayAlert("Error", $"An error occurred: {ex.Message}", "OK");
			return;
		}
	}
	#endregion

	//-----------------------------------------------------------------------------------------------

	#region methods
	public class SearchFilter
	{
		public string label { get; set; }
		public string value { get; set; }
		public int ID { get; set; }
	}
	public async Task<bool> SearchItemBySKU(string SearchValue)
	{
		bool ExtendedSearch = false;
		bool UPC_EToA = false;
		string NonPLUFlag = "2";
		string DateMask = "";
		DBManager _DbMgr = Globals.DBServer;
		string StoreID = Globals.Store_ID;

		try
		{
			listSection.IsVisible = false;
			detailSection.IsVisible = true;
			//CheckScrollBarVisibility();
			//SearchEntry.Text = "";
			Items.Clear();
			loadingpopup.Hide();

			#region Set up Values
			SetupValue_dal Setup_dal = new SetupValue_dal(_DbMgr, true);
			OptionConfigValue _SvValue = Setup_dal.SetupOptionValue;
			int nRes = -1;
			bool DoNotSearchItemBySKU = false;
			nRes = Setup_dal.GetSetupOption(StoreID, "EXTENDEDSEARCH");
			if (nRes == 1)
			{
				ExtendedSearch = _SvValue.numValue == 1 ? true : false;
			}
			nRes = Setup_dal.GetSetupOption(StoreID, "UPC_ETOA");
			if (nRes == 1)
			{
				UPC_EToA = _SvValue.numValue == 1 ? true : false;
			}
			nRes = Setup_dal.GetSetupOption(StoreID, "NONPLUFLAG");
			if (nRes == 1)
			{
				NonPLUFlag = _SvValue.strValue;
			}
			nRes = Setup_dal.GetSetupOption(StoreID, "DATEFORMAT");
			if (nRes == 1)
			{
				DateMask = _SvValue.strValue;
			}
			nRes = Setup_dal.GetSetupOption(StoreID, "DATEFONOSEARCHBYSKURMAT");
			if (nRes == 1)
			{
				DoNotSearchItemBySKU = _SvValue.numValue == 1 ? true : false;
			}

			#endregion


			ItemSearchRequest objSearch = new ItemSearchRequest(SearchValue, false);
			objSearch.FixUPC = false;
			objSearch.PerformExtendedSearch = ExtendedSearch;
			objSearch.ShowMultiPackDialog = false;
			ItemSearchResponse objResult = null;

			bool bFound = SearchUPC(Globals.DBServer, UPC_EToA, NonPLUFlag, objSearch, out objResult);

			if (bFound == false)
			{
				// loadingpopup.Hide();
				detailSection.IsVisible = false;
				introSection.IsVisible = true;
				lblneedprice.Text = "Item not Found!";
				lblsimplytext.IsVisible = false;
				//DisplayAlert("Alert!!", "Item Not Found", "OK");
				return false;
			}

			if (bFound == false || objResult == null || objResult.IsSKUFound == false)
			{
				// loadingpopup.Hide();
				detailSection.IsVisible = false;
				introSection.IsVisible = true;
				lblneedprice.Text = "Item not Found!";
				lblsimplytext.IsVisible = false;
				//DisplayAlert("Alert!!", "Item Not Found", "OK");
				return false;
			}
			else
			{
				SearchValue = objResult.FoundSKU.ToString();
			}

			Item_DAL _ItemDAL = null;
			string ErrMsg = string.Empty;
			if (!Item_DAL.Instantiate(_DbMgr, false, out ErrMsg, out _ItemDAL))
			{
				DisplayAlert("Alert!!", ErrMsg, "OK");
				return false;
			}

			int SKU = 0;
			string ItemName = "";
			string SizeName = "";
			string PackName = "";
			string DepName = "";
			string CatName = "";
			decimal RegPrice = 0.00m;
			string Location = "";
			string Description = "";
			string CurrencySymbol = "";
			string strUPC = "";

			nRes = Setup_dal.GetSetupOption(StoreID, "CURRSYMBOL");
			if (nRes == 1)
			{ CurrencySymbol = _SvValue.strValue; }

			CurrencySymbol1.Text = CurrencySymbol;
			//CurrencySymbol2.Text = CurrencySymbol;

			DataTable dt = _ItemDAL.GetItemByCriteria(SearchValue, 0, 0, true);

			if (dt.Rows.Count == 0)
			{
				//DisplayAlert("Alert!!", "Item Not Found", "OK");
				detailSection.IsVisible = false;
				introSection.IsVisible = true;
				lblneedprice.Text = "Item not Found!";
				lblsimplytext.IsVisible = false;
				return false;
			}

			SKU = System.Convert.ToInt32(dt.Rows[0]["SKU"].ToString());
			ItemName = dt.Rows[0]["ITEMNAME"].ToString() != null ? dt.Rows[0]["ITEMNAME"].ToString() : "";
			SizeName = dt.Rows[0]["SizeName"].ToString() != null ? dt.Rows[0]["SizeName"].ToString() : "";
			PackName = dt.Rows[0]["PackName"].ToString() != null ? dt.Rows[0]["PackName"].ToString() : "";

			DepName = dt.Rows[0]["DepName"].ToString() != null ? dt.Rows[0]["DepName"].ToString() : "";
			CatName = dt.Rows[0]["CatName"].ToString() != null ? dt.Rows[0]["CatName"].ToString() : "";
			RegPrice = System.Convert.ToDecimal(dt.Rows[0]["PricePerUnit"].ToString());
			Description = dt.Rows[0]["Description"].ToString();
			strUPC = dt.Rows[0]["MAINUPC"].ToString();

			string sqlQuery = @"SELECT SI.SALEPRICE, SI.QUANTITY
                 FROM PRM_SALEITEM SI INNER JOIN PRM_SALE S ON S.SALEID = SI.SALEID
                 WHERE SI.SKU = " + SKU + @" AND S.STOREID = '" + StoreID + @"' AND
                 (EndDate IS NULL OR 
                     (
                         CAST(EndDate AS DATE) >= CAST(GETDATE() AS DATE) 
                         AND CAST(StartDate AS DATE) <= CAST(GETDATE() AS DATE)
                     )
                 )
                 ORDER BY SEQUENCE DESC LIMIT 1";

			#region Sale Price

			SALE_DAL _SaleDAL = new SALE_DAL(_DbMgr, false);
			string SalePrice = _SaleDAL.LoadSaleBySKU(StoreID, SKU, CurrencySymbol, true, true);
			if (!string.IsNullOrEmpty(SalePrice))
				//SalePrice = SalePrice.Replace("\r\n", "<br/>");
				SalePrice = SalePrice.Replace("\r\n", "\n");

			#endregion

			DataTable dtLocation = _DbMgr.GetDataTable(@"SELECT TOP 1 SA.STORAGEAREANAME
             FROM ITM_STORAGEAREA ISA INNER JOIN STK_STORAGEAREA SA ON ISA.STORAGEAREAID = SA.STORAGEAREAID
             WHERE SKU = " + SKU);
			if (dtLocation != null && dtLocation.Rows.Count > 0)
				Location = dtLocation.Rows[0]["STORAGEAREANAME"].ToString();
			else
				Location = "";

			//string strUPC = "";
			//DataTable dtUPC = _ItemDAL.GetItemUPCByItem(SKU);
			//if (dtUPC != null && dtUPC.Rows.Count > 0)
			//{
			//	foreach (DataRow item in dtUPC.Rows)
			//	{
			//		strUPC += item["UPC"].ToString() + "\n";
			//	}
			//}

			//if (!string.IsNullOrEmpty(strUPC))
			//{
			//	strUPC = strUPC.Substring(0, strUPC.Length - 2);
			//}

			//return Json(Globals.ReturnResult(new
			//{
			//	ItemName = ItemName,
			//	SizeName = SizeName,
			//	PackName = PackName,
			//	DepName = DepName,
			//	CatName = CatName,
			//	RegPrice = RegPrice,
			//	SalePrice = SalePrice,
			//	Location = Location,
			//	Description = Description,
			//	CurrencySymbol = CurrencySymbol,
			//	strUPC = strUPC
			//}), JsonRequestBehavior.AllowGet);
			lblName.Text = ItemName;

			//lblSKU.Text = SKU.ToString();
			if (SizeName == "")
			{
				//lblSizeName.IsVisible = false;
				valuesizeName.Text = "          -";
			}
			else
			{
				lblSizeName.IsVisible = true;
				valuesizeName.Text = SizeName;
			}

			if (PackName == "")
			{
				//lblPackName.IsVisible = false;
				valuePackName.Text = "          -";
			}
			else
			{
				lblPackName.IsVisible = true;
				valuePackName.Text = PackName;
			}

			if (DepName == "")
			{
				//lblDepName.IsVisible = false;
				valueDepName.Text = "          -";
			}
			else
			{
				lblDepName.IsVisible = true;
				valueDepName.Text = DepName;
			}

			if (CatName == "")
			{
				//lblCatName.IsVisible = false;
				valueCatName.Text = "          -";
			}
			else
			{
				lblCatName.IsVisible = true;
				valueCatName.Text = CatName;
			}

			if (RegPrice == 0)
			{
				lblRegPrice.IsVisible = false;
			}
			else
			{
				lblRegPrice.IsVisible = true;
				valueRegPrice.Text = RegPrice.ToString();
			}

			if (SalePrice == "")
			{
				lblSalePrice.IsVisible = false;
			}
			else
			{
				lblSalePrice.IsVisible = true;
				valueSalePrice.Text = SalePrice;
			}

			if (Location == "")
			{
				//lblLocation.IsVisible = false;
				valueLocation.Text = "          -";
			}
			else
			{
				lblLocation.IsVisible = true;
				valueLocation.Text = Location;

			}

			if (Description == "")
			{
				//lblDescription.IsVisible = false;
				valueDescription.Text = "          -";
			}
			else
			{
				lblDescription.IsVisible = true;
				valueDescription.Text = Description;
			}

			if (strUPC == "")
			{
				lblstrUPC.IsVisible = false;
			}
			else
			{
				lblstrUPC.IsVisible = true;
				valuestrUPC.Text = strUPC;
			}

			return true;

		}
		catch (Exception ex)
		{
			await DisplayAlert("Alert!!", ex.Message, "OK");
			return false;
		}
		finally
		{
			SearchEntry.Text = "";
		}
		//finally
		//{
		//	if (DbMgr?.DBConn != null && DbMgr.DBConn.State != ConnectionState.Closed)
		//		_DbMgr.CloseConnection();
		//}
	}
	public bool SearchUPC(DBManager db, bool UPC_EtoA, string NonPLUFlag, ItemSearchRequest objSearchReq, out ItemSearchResponse objResult)
	{
		objResult = null;

		Item_DAL _ItemDAL = null;
		string ErrMsg = string.Empty;
		if (!Item_DAL.Instantiate(db, false, out ErrMsg, out _ItemDAL))
		{
			return false;
		}
		if (Globals.ConnectToDatabase(true) == false)
		{
			Globals.IsServerConnected = false;
			SearchEntry.Text = "";
			DisplayAlert("No Internet Connection!", "Unable to connect to database.", "ok");
			SearchEntry.Focus();
			return false;
		}

		int ControlSKU = 0;
		bool ShowDialog = false;
		int MatrixSKU = 0;
		int ScanCodeID = 0;
		string ModifiedUPC = null;

		string objSearchReq_SearchString = objSearchReq.SearchString;
		string objSearchReq_SearchUPC_EtoA = "";

		if (UPC_EtoA)
		{
			if (objSearchReq_SearchString.Length >= 6 && objSearchReq_SearchString.Length <= 8)
			{
				#region Convert UPC-E to UPC-A

				string SearchString = BarcodeConverter.UPCE2UPCA(objSearchReq_SearchString);
				if (SearchString != null && !string.IsNullOrEmpty(SearchString))
				{
					objSearchReq_SearchUPC_EtoA = SearchString;
					objSearchReq.FixUPC = false;
				}

				#endregion
			}
		}

		int resSKU = _ItemDAL.SearchItemByUPC(objSearchReq_SearchString, objSearchReq_SearchUPC_EtoA, out ModifiedUPC, out ControlSKU, out ShowDialog, out MatrixSKU, out ScanCodeID, objSearchReq.PerformExtendedSearch, objSearchReq.FixUPC, NonPLUFlag);
		if (resSKU == -1)
		{
			return false;
		}
		else if (resSKU == 0 && ScanCodeID == 0) //--- item not found
		{
			return false;
		}
		else
		{
			objResult = new ItemSearchResponse();
			objResult.IsSKUFound = true;
			objResult.FoundSKU = resSKU;

			if (objSearchReq.ShowMultiPackDialog == true && ControlSKU > 0 && ShowDialog == true)
				objResult.ControlSKU = ControlSKU;
			else
				objResult.ControlSKU = 0;

			if (MatrixSKU > 0)
				objResult.MatrixSKU = MatrixSKU;
			else
				objResult.MatrixSKU = 0;
			objResult.ScanCodeID = ScanCodeID;

			return true;
		}
	}
	public bool SearchByKeyword(string StoreConfig, string SearchTerm)
	{
		DBManager _DbMgr = Globals.DBServer;
		try
		{
			string StoreID = Globals.Store_ID;
			//_DbMgr = db.GetDbManager(StoreConfig, out StoreID);
			if (_DbMgr == null)
			{
				DisplayAlert("Alert!!", _DbMgr.GetErrorMessage(), "OK");
				return false;
			}

			TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
			List<SearchFilter> lstFilterResult = new List<SearchFilter>();

			ModelGlobals.STORENUM = 0;
			ModelGlobals.ZONEID = 1;
			ModelGlobals.ZONENUM = 0;

			//try
			//{
			//	if (!System.Convert.IsDBNull(HttpContext.Cache["STORENUM"]) && !string.IsNullOrEmpty(HttpContext.Cache["STORENUM"].ToString()))
			//		ModelGlobals.STORENUM = ((int)HttpContext.Cache["STORENUM"]);
			//	if (!System.Convert.IsDBNull(HttpContext.Cache["ZONEID"]) && !string.IsNullOrEmpty(HttpContext.Cache["ZONEID"].ToString()))
			//		ModelGlobals.ZONEID = ((int)HttpContext.Cache["ZONEID"]);
			//	if (!System.Convert.IsDBNull(HttpContext.Cache["ZONENUM"]) && !string.IsNullOrEmpty(HttpContext.Cache["ZONENUM"].ToString()))
			//		ModelGlobals.ZONENUM = ((int)HttpContext.Cache["ZONENUM"]);
			//}
			//catch { }

			//string Col_Qty = ModelGlobals.Get_TotalQty();
			//string ColPricePerUnitWhere = "";
			//string ColPricePerUnit = Globals.GetColPricePerUnit(out ColPricePerUnitWhere);

			string sql = @"SELECT (CASE WHEN DONOTPROMPTMTX = true THEN I.MATRIXNAME  
	                                   ELSE CONCAT(I.ITEMNAME, ' ', COALESCE(S.SIZENAME, ''), ' ', COALESCE(P.PACKNAME, ''))
	                              END) AS ITEMNAME, 
	                           SKU
	                           FROM ITM_ITEMINFO I
	                           LEFT JOIN ITM_ITEMSIZE S ON S.SIZEID = I.SIZEID
	                           LEFT JOIN ITM_ITEMPACK P ON P.PACKID = I.PACKID
	                           WHERE (I.NONACTIVE = 0 OR I.NONACTIVE IS NULL) 
	                           AND I.ITEMLEVEL = (CASE WHEN I.DONOTPROMPTMTX = true THEN 0 ELSE I.ITEMLEVEL END)
	                           AND I.KEYWORD ILIKE '%" + SearchTerm.Replace("'", "\'\'") + @"%'";

			DataTable dbproduct = _DbMgr.GetDataTable(sql);

			if (dbproduct == null)
			{
				DisplayAlert("Alert!!", _DbMgr.GetErrorMessage(), "OK");
				return false;
			}
			SearchFilter objFilter = null;

			for (int i = 0; i < dbproduct.Rows.Count; i++)
			{
				objFilter = new SearchFilter();
				objFilter.label = dbproduct.Rows[i]["ITEMNAME"].ToString();
				objFilter.value = dbproduct.Rows[i]["ITEMNAME"].ToString();
				objFilter.ID = System.Convert.ToInt32(dbproduct.Rows[i]["SKU"].ToString());
				Items.Add(objFilter);
			}
			MyCollectionView.ItemsSource = Items;
			return true;
		}
		catch (Exception ex)
		{
			DisplayAlert("Error", $"An error occurred: {ex.Message}", "OK");
			return false;
		}
		finally
		{
			if (_DbMgr?.DBConn != null && _DbMgr.DBConn.State != ConnectionState.Closed)
				_DbMgr.CloseConnection();
		}
	}
	#endregion

	//public void CheckScrollBarVisibility()
	//{
	//	// Wait for layout cycle to complete
	//	detailSection.SizeChanged += (s, e) =>
	//	{
	//		// Compare content size to ScrollView's visible size
	//		bool isVerticalScrollBarVisible = detailSection.ContentSize.Height > detailSection.Height;


	//		if (isVerticalScrollBarVisible)
	//		{
	//			Console.WriteLine("Vertical scrollbar will be visible.");
	//		}


	//	};
	//}

	private async void SizeEvaluation()
	{
		try
		{
			//await DisplayAlert("Data", $"detailSection.ContentSize.Height: {detailSection.ContentSize.Height}\ndetailSection.Height : {detailSection.Height}", "OK");
			await Task.Delay(100); // Small delay to allow the layout to stabilize
			if (detailSection.ContentSize.Height > detailSection.Height)
			{
				scrollToBottomButton.IsVisible = true;
				//scrollToTopButton.IsVisible = true;
			}
			else
			{
				// Disable the button if no scrolling is required
				scrollToBottomButton.IsVisible = false;
				scrollToTopButton.IsVisible = false;
			}
		}
		catch (Exception ex)
		{
			DisplayAlert("Error", $"An error occurred: {ex.Message}", "OK");
			return;
		}
	}

	//private async void detailSection_Scrolled(object sender, ScrolledEventArgs e)
	//{
	//	try
	//	{
	//		if (bIsScrollBtnClicked)
	//		{
	//			return;
	//		}
	//		await Task.Delay(100);
	//		detailSection_SizeChanged(sender, e);
	//	}
	//	catch (Exception ex)
	//	{
	//		await DisplayAlert("Error", $"An error occurred: {ex.Message}", "OK");
	//		return;
	//	}
	//}
	private async void detailSection_Scrolled(object sender, ScrolledEventArgs e)
	{
		try
		{
			// TIMER
			//ResetTimer();
			if (bIsScrollBtnClicked)
			{
				return;
			}
			// TIMER
			ResetTimer();
			// Check if the user has scrolled to the bottom
			if (Math.Abs(detailSection.ScrollY - (detailSection.ContentSize.Height - detailSection.Height)) < 1)
			{
				scrollToBottomButton.IsVisible = false;
				scrollToTopButton.IsVisible = true;
			}
			// Check if the user has scrolled to the top
			else if (detailSection.ScrollY <= 0)
			{
				scrollToTopButton.IsVisible = false;
				scrollToBottomButton.IsVisible = true;
			}
			else
			{
				// Show both buttons when not at the top or bottom
				scrollToBottomButton.IsVisible = true;
				scrollToTopButton.IsVisible = true;
			}
		}
		catch (Exception ex)
		{
			await DisplayAlert("Error", $"An error occurred: {ex.Message}", "OK");
			return;
		}
	}


	private void detailSection_SizeChanged(object sender, EventArgs e)
	{
		try
		{
			Task.Delay(100);
			detailSection.ForceLayout(); // Force a layout update

			Dispatcher.Dispatch(() =>
			{
				Console.WriteLine($"detailSection.ContentSize.Height: {detailSection.ContentSize.Height}, detailSection.Height: {detailSection.Height}");
				bool isVerticalScrollBarVisible = detailSection.ContentSize.Height > detailSection.Height;

				if (isVerticalScrollBarVisible)
				{
					// Enable the scroll-down button
					scrollToBottomButton.IsVisible = true;
					scrollToTopButton.IsVisible = false;
				}
				else
				{
					// Disable the button if no scrolling is required
					scrollToBottomButton.IsVisible = false;
					scrollToTopButton.IsVisible = false;
				}
			});
		}
		catch (Exception ex)
		{
			DisplayAlert("Error", $"An error occurred: {ex.Message}", "OK");
			return;
		}
	}


	private async void ScrollToTopButton_Clicked(object sender, EventArgs e)
	{
		try
		{
			bIsScrollBtnClicked = true;
			// Scroll to the top of the content
			await detailSection.ScrollToAsync(0, 0, true);
			scrollToTopButton.IsVisible = false;
			scrollToBottomButton.IsVisible = true;
			bIsScrollBtnClicked = false;
			// TIMER
			ResetTimer();
		}
		catch (Exception ex)
		{
			await DisplayAlert("Error", $"An error occurred: {ex.Message}", "OK");
			return;
		}
	}

	private async void ScrollToBottomButton_Clicked(object sender, EventArgs e)
	{
		try
		{
			bIsScrollBtnClicked = true;
			// Scroll to the bottom of the content
			await detailSection.ScrollToAsync(0, detailSection.ContentSize.Height, true);
			scrollToBottomButton.IsVisible = false;
			scrollToTopButton.IsVisible = true;
			bIsScrollBtnClicked = false;
			// TIMER
			ResetTimer();
		}
		catch (Exception ex)
		{
			await DisplayAlert("Error", $"An error occurred: {ex.Message}", "OK");
			return;
		}
	}



}