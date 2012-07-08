using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.IO;
using System.Windows.Forms;
using System.Data;
using Yahoo.API;

namespace TestHarness
{
	/// <summary>
	/// Summary description for Form1.
	/// </summary>
	public class TestHarness : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Button btnSearch;
		private System.Windows.Forms.TextBox txtResult;
		private System.Windows.Forms.Button btnRelatedSuggestions;
		private System.Windows.Forms.Button btnSpelling;
		private System.Windows.Forms.Button btnVideo;
		private System.Windows.Forms.Button btnNews;
		private System.Windows.Forms.Button btnLocal;
		private System.Windows.Forms.Button btnImage;
		private System.Windows.Forms.Button btnTermExtraction;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public TestHarness()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if (components != null) 
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.btnSearch = new System.Windows.Forms.Button();
			this.txtResult = new System.Windows.Forms.TextBox();
			this.btnRelatedSuggestions = new System.Windows.Forms.Button();
			this.btnSpelling = new System.Windows.Forms.Button();
			this.btnVideo = new System.Windows.Forms.Button();
			this.btnNews = new System.Windows.Forms.Button();
			this.btnLocal = new System.Windows.Forms.Button();
			this.btnImage = new System.Windows.Forms.Button();
			this.btnTermExtraction = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// btnSearch
			// 
			this.btnSearch.Location = new System.Drawing.Point(16, 16);
			this.btnSearch.Name = "btnSearch";
			this.btnSearch.TabIndex = 0;
			this.btnSearch.Text = "Search";
			this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
			// 
			// txtResult
			// 
			this.txtResult.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.txtResult.Location = new System.Drawing.Point(16, 80);
			this.txtResult.Multiline = true;
			this.txtResult.Name = "txtResult";
			this.txtResult.ScrollBars = System.Windows.Forms.ScrollBars.Both;
			this.txtResult.Size = new System.Drawing.Size(560, 284);
			this.txtResult.TabIndex = 1;
			this.txtResult.Text = "";
			this.txtResult.WordWrap = false;
			// 
			// btnRelatedSuggestions
			// 
			this.btnRelatedSuggestions.Location = new System.Drawing.Point(96, 16);
			this.btnRelatedSuggestions.Name = "btnRelatedSuggestions";
			this.btnRelatedSuggestions.TabIndex = 2;
			this.btnRelatedSuggestions.Text = "Related Suggestions";
			this.btnRelatedSuggestions.Click += new System.EventHandler(this.btnRelatedSuggestions_Click);
			// 
			// btnSpelling
			// 
			this.btnSpelling.Location = new System.Drawing.Point(176, 16);
			this.btnSpelling.Name = "btnSpelling";
			this.btnSpelling.TabIndex = 3;
			this.btnSpelling.Text = "Spelling";
			this.btnSpelling.Click += new System.EventHandler(this.btnSpelling_Click);
			// 
			// btnVideo
			// 
			this.btnVideo.Location = new System.Drawing.Point(256, 16);
			this.btnVideo.Name = "btnVideo";
			this.btnVideo.TabIndex = 4;
			this.btnVideo.Text = "Video";
			this.btnVideo.Click += new System.EventHandler(this.btnVideo_Click);
			// 
			// btnNews
			// 
			this.btnNews.Location = new System.Drawing.Point(16, 48);
			this.btnNews.Name = "btnNews";
			this.btnNews.TabIndex = 5;
			this.btnNews.Text = "News";
			this.btnNews.Click += new System.EventHandler(this.btnNews_Click);
			// 
			// btnLocal
			// 
			this.btnLocal.Location = new System.Drawing.Point(96, 48);
			this.btnLocal.Name = "btnLocal";
			this.btnLocal.TabIndex = 6;
			this.btnLocal.Text = "Local";
			this.btnLocal.Click += new System.EventHandler(this.btnLocal_Click);
			// 
			// btnImage
			// 
			this.btnImage.Location = new System.Drawing.Point(176, 48);
			this.btnImage.Name = "btnImage";
			this.btnImage.TabIndex = 7;
			this.btnImage.Text = "Image";
			this.btnImage.Click += new System.EventHandler(this.btnImage_Click);
			// 
			// btnTermExtraction
			// 
			this.btnTermExtraction.Location = new System.Drawing.Point(256, 48);
			this.btnTermExtraction.Name = "btnTermExtraction";
			this.btnTermExtraction.TabIndex = 8;
			this.btnTermExtraction.Text = "Term Extrac";
			this.btnTermExtraction.Click += new System.EventHandler(this.btnTermExtraction_Click);
			// 
			// TestHarness
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(584, 374);
			this.Controls.Add(this.btnTermExtraction);
			this.Controls.Add(this.btnImage);
			this.Controls.Add(this.btnLocal);
			this.Controls.Add(this.btnNews);
			this.Controls.Add(this.btnVideo);
			this.Controls.Add(this.btnSpelling);
			this.Controls.Add(this.btnRelatedSuggestions);
			this.Controls.Add(this.txtResult);
			this.Controls.Add(this.btnSearch);
			this.Name = "TestHarness";
			this.Text = "Yahoo Search API Example";
			this.ResumeLayout(false);

		}
		#endregion

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main() 
		{
			Application.Run(new TestHarness());
		}

		private void btnSearch_Click(object sender, System.EventArgs e)
		{
			YahooSearchService yahoo = new YahooSearchService();

			Yahoo.API.WebSearchResponse.ResultSet resultSet = yahoo.WebSearch("YahooExample", "site:www.mgbrown.com", "all", 10, 1, "any", true, true, "en");

			StringWriter sw = new StringWriter();
			foreach (Yahoo.API.WebSearchResponse.ResultType result in resultSet.Result)
			{
				sw.WriteLine("Title: {0}", result.Title);
				sw.WriteLine("Summary: {0}", result.Summary);
				sw.WriteLine("URL: {0}", result.Url);
				sw.WriteLine("===============================================================");
			}

			txtResult.Text = sw.ToString();
		}

		private void btnRelatedSuggestions_Click(object sender, System.EventArgs e)
		{
			YahooSearchService yahoo = new YahooSearchService();

			Yahoo.API.WebSearchRelatedResponse.ResultSet resultSet = yahoo.WebSearchRelated("YahooExample", "madonna", 10);

			StringWriter sw = new StringWriter();
			foreach (string result in resultSet.Result)
			{
				sw.WriteLine("Related Suggestion: {0}", result);
				sw.WriteLine("===============================================================");
			}

			txtResult.Text = sw.ToString();
		}

		private void btnSpelling_Click(object sender, System.EventArgs e)
		{
			YahooSearchService yahoo = new YahooSearchService();

			Yahoo.API.WebSearchSpellingResponse.ResultSet resultSet = yahoo.WebSearchSpelling("YahooExample", "madnna");

			StringWriter sw = new StringWriter();
			sw.WriteLine("Spelling Suggestion: {0}", resultSet.Result);

			txtResult.Text = sw.ToString();
		}

		private void btnVideo_Click(object sender, System.EventArgs e)
		{
			YahooSearchService yahoo = new YahooSearchService();

			Yahoo.API.VideoSearchResponse.ResultSet resultSet = yahoo.VideoSearch("YahooExample", "madonna", "all", 10, 1, "any", true);

			StringWriter sw = new StringWriter();
			foreach (Yahoo.API.VideoSearchResponse.ResultType result in resultSet.Result)
			{
				sw.WriteLine("Title: {0}", result.Title);
				sw.WriteLine("Summary: {0}", result.Summary);
				sw.WriteLine("URL: {0}", result.Url);
				sw.WriteLine("===============================================================");
			}

			txtResult.Text = sw.ToString();
		}

		private void btnNews_Click(object sender, System.EventArgs e)
		{
			YahooSearchService yahoo = new YahooSearchService();

			Yahoo.API.NewsSearchResponse.ResultSet resultSet = yahoo.NewsSearch("YahooExample", "madonna", "all", 10, 1, "rank", "en");

			StringWriter sw = new StringWriter();
			foreach (Yahoo.API.NewsSearchResponse.ResultType result in resultSet.Result)
			{
				sw.WriteLine("Title: {0}", result.Title);
				sw.WriteLine("Summary: {0}", result.Summary);
				sw.WriteLine("URL: {0}", result.Url);
				sw.WriteLine("===============================================================");
			}

			txtResult.Text = sw.ToString();
		}

		private void btnLocal_Click(object sender, System.EventArgs e)
		{
			YahooSearchService yahoo = new YahooSearchService();

			Yahoo.API.LocalSearchResponse.ResultSet resultSet = yahoo.LocalSearch("YahooExample", "pizza", 10, 1, (float)10.0, "", "", "", "94306", "");

			StringWriter sw = new StringWriter();
			foreach (Yahoo.API.LocalSearchResponse.ResultType result in resultSet.Result)
			{
				sw.WriteLine("Title: {0}", result.Title);
				sw.WriteLine("Address: {0}", result.Address);
				sw.WriteLine("URL: {0}", result.Url);
				sw.WriteLine("===============================================================");
			}

			txtResult.Text = sw.ToString();
		}

		private void btnImage_Click(object sender, System.EventArgs e)
		{
			YahooSearchService yahoo = new YahooSearchService();

			Yahoo.API.ImageSearchResponse.ResultSet resultSet = yahoo.ImageSearch("YahooExample", "maddona", "all", 10, 1, "any", true );

			StringWriter sw = new StringWriter();
			foreach (Yahoo.API.ImageSearchResponse.ResultType result in resultSet.Result)
			{
				sw.WriteLine("Title: {0}", result.Title);
				sw.WriteLine("Summary: {0}", result.Summary);
				sw.WriteLine("URL: {0}", result.Url);
				sw.WriteLine("===============================================================");
			}

			txtResult.Text = sw.ToString();
		}

		private void btnTermExtraction_Click(object sender, System.EventArgs e)
		{
			YahooSearchService yahoo = new YahooSearchService();

			Yahoo.API.TermExtractionResponse.ResultSet resultSet = yahoo.TermExtraction("YahooExample", "maddona", "Italian sculptors and painters of the renaissance favored the Virgin Mary for inspiration.");

			StringWriter sw = new StringWriter();
			foreach (string result in resultSet.Result)
			{
				sw.WriteLine("{0}", result);
			}

			txtResult.Text = sw.ToString();
		}
	}
}
