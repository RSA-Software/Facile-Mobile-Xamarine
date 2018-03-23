﻿using System;
using System.IO;
using System.Net;
using Facile.Interfaces;
using Facile.Droid.Interfaces;
using System.Threading.Tasks;
using System.Collections.Generic;

[assembly: Xamarin.Forms.Dependency(typeof(FTP))] //You need to put this on iOS/droid class or uwp/etc if you wrote
namespace Facile.Droid.Interfaces
{
	class FTP : IFtpWebRequest
	{
		public FTP() //I saw on Xamarin documentation that it's important to NOT pass any parameter on that constructor
		{
		}

		/// Upload File to Specified FTP Url with username and password and Upload Directory if need to upload in sub folders
		///Base FtpUrl of FTP Server
		///Local Filename to Upload
		///Username of FTP Server
		///Password of FTP Server
		///[Optional]Specify sub Folder if any
		/// Status String from Server
		public async Task<string> UploadFile(string FtpUrl, string fileName, string userName, string password, string UploadDirectory = "")
		{
			return await Task.Run(() =>
			{
				try
				{

					string PureFileName = new FileInfo(fileName).Name;
					String uploadUrl = String.Format("{0}{1}/{2}", FtpUrl, UploadDirectory, PureFileName);
					FtpWebRequest req = (FtpWebRequest)WebRequest.Create(uploadUrl);
					req.Proxy = null;
					req.Method = WebRequestMethods.Ftp.UploadFile;
					req.Credentials = new NetworkCredential(userName, password);
					req.UseBinary = true;
					req.UsePassive = true;
					byte[] data = File.ReadAllBytes(fileName);
					req.ContentLength = data.Length;
					Stream stream = req.GetRequestStream();
					stream.Write(data, 0, data.Length);
					stream.Close();
					FtpWebResponse res = (FtpWebResponse)req.GetResponse();
					return res.StatusDescription;

				}
				catch (Exception err)
				{
					return err.ToString();
				}
			});
		}

		public async Task<string> DownloadFile(string userName, string password, string ftpSourceFilePath, string localDestinationFilePath)
		{
			return await Task.Run(() =>
			{
				try
				{
					int bytesRead = 0;
					byte[] buffer = new byte[2048];

					FtpWebRequest req = (FtpWebRequest)WebRequest.Create(ftpSourceFilePath);
					req.Method = WebRequestMethods.Ftp.DownloadFile;
					req.Proxy = null;
					req.Credentials = new NetworkCredential(userName, password);
					req.UseBinary = true;
					req.UsePassive = true;

					Stream reader = req.GetResponse().GetResponseStream();
					FileStream fileStream = new FileStream(localDestinationFilePath, FileMode.Create);

					while (true)
					{
						bytesRead = reader.Read(buffer, 0, buffer.Length);

						if (bytesRead == 0)
							break;

						fileStream.Write(buffer, 0, bytesRead);
					}
					fileStream.Close();
					FtpWebResponse res = (FtpWebResponse)req.GetResponse();
					return res.StatusDescription;
				}
				catch (Exception err)
				{
					return err.ToString();
				}
			});
		}

		public async Task<List<string>> ListDirectory(string userName, string password, string ftpServer)
		{
			return await Task.Run(() =>
			{
				List<string> files = new List<string>();

				try
				{
					//Create FTP request
					FtpWebRequest req = (FtpWebRequest)WebRequest.Create(ftpServer);
					req.Method = WebRequestMethods.Ftp.ListDirectory;
					req.Credentials = new NetworkCredential(userName, password);
					req.UsePassive = true;
					req.UseBinary = true;
					req.KeepAlive = false;

					FtpWebResponse response = (FtpWebResponse)req.GetResponse();
					Stream responseStream = response.GetResponseStream();
					StreamReader reader = new StreamReader(responseStream);

					while (!reader.EndOfStream)
					{
						//Application.DoEvents();
						files.Add(reader.ReadLine());
					}

					//Clean-up
					reader.Close();
					responseStream.Close(); //redundant
					response.Close();

					return files;
				}
				catch (Exception)
				{
					return null;
				}
			});
		}

	}
}
