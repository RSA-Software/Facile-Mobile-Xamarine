﻿using Xamarin.Forms;
using Facile.Interfaces;
using Facile.Models;
using SQLite;
using System;
using static Facile.Extension.FattureExtensions;

namespace Facile
{
	public partial class FacilePage : ContentPage
	{
		void OnClickedClienti(object sender, System.EventArgs e)
		{
			Navigation.PushAsync(new ArticoliSearch());
		}


		async void OnClickedOrdini(object sender, System.EventArgs e)
		{
			await Navigation.PushAsync(new FatturePage(TipoDocumento.TIPO_ORD));
		}



		async void OnClickedCarrello(object sender, System.EventArgs e)
		{
			var page = new DocumentiGrid(TipoDocumento.TIPO_FAT);
			await Navigation.PushAsync(page);
		}

		async void OnClickedCatalogo(object sender, System.EventArgs e)
		{
			Fatture fat = null;
			SQLiteAsyncConnection dbcon_ = DependencyService.Get<ISQLiteDb>().GetConnection();

			bool nuova = false;
			try
			{
				var docList = await dbcon_.QueryAsync<Fatture>("SELECT * from FATTURE2 WHERE fat_tipo = 0 AND fat_n_doc = 5228 LIMIT 1");

				foreach (var doc in docList)
				{
					fat = doc;
					break;
				}
			}
			catch (Exception ex)
			{
				await DisplayAlert("Attenzione!", ex.Message, "OK");
				return;
			}
			if (fat == null) return;

			var page = new DocumentiEdit(ref fat, ref nuova);
			await Navigation.PushAsync(page);
		}

		async void OnClickedScadenze(object sender, System.EventArgs e)
		{
			var page = new ScadenzeElenco();
			await Navigation.PushAsync(page);
		}

		void OnClickedIncassi(object sender, System.EventArgs e)
		{
			DisplayAlert("Incassi", "Clicked", "ok");
		}

		async void OnClickedSincronizza(object sender, System.EventArgs e)
		{
			//await Navigation.PushAsync(new DownloadPage());
			await Navigation.PushModalAsync(new DownloadPage());
		}

		async void OnClickedImpostazioni(object sender, System.EventArgs e) => await Navigation.PushAsync(new SetupPage());

		public FacilePage()
		{
			InitializeComponent();

		}

		protected override async void OnAppearing()
		{
			//DependencyService.Get<ISQLiteDb>().RemoveDB();
			var dbcon = DependencyService.Get<ISQLiteDb>().GetConnection();

			await dbcon.CreateTableAsync<Ditte>();
			await dbcon.CreateTableAsync<Zone>();
			await dbcon.CreateTableAsync<Cateco>();
			await dbcon.CreateTableAsync<Pagamenti>();
			await dbcon.CreateTableAsync<Tabelle>();
			await dbcon.CreateTableAsync<Agenti>();
			await dbcon.CreateTableAsync<Misure>();
			await dbcon.CreateTableAsync<Clienti>();
			await dbcon.CreateTableAsync<Destinazioni>();
			await dbcon.CreateTableAsync<Scadenze>();
			await dbcon.CreateTableAsync<Codiva>();
			await dbcon.CreateTableAsync<Reparti>();
			await dbcon.CreateTableAsync<Catmerc>();
			await dbcon.CreateTableAsync<Fornitori>();
			await dbcon.CreateTableAsync<Depositi>();
			await dbcon.CreateTableAsync<Lotti>();
			await dbcon.CreateTableAsync<Artanag>();
			await dbcon.CreateTableAsync<Listini>();
			await dbcon.CreateTableAsync<Fatture>();
			await dbcon.CreateTableAsync<FatRow>();
			await dbcon.CreateTableAsync<Vettori>();
			await dbcon.CreateTableAsync<Banche>();
			await dbcon.CreateTableAsync<Canali>();
			await dbcon.CreateTableAsync<Stagioni>();
			await dbcon.CreateTableAsync<Marchi>();
			await dbcon.CreateTableAsync<Associazioni>();

			base.OnAppearing();
		}
	}
}
