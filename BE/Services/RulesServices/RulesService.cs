using AutoMapper;
using BE.Data.Contexts;
using BE.Data.Dtos.PaidDtos;
using BE.Data.Dtos.RulesDTOs;
using BE.Data.Models;
using BE.Helpers;
using BE.Response;
using BE.shared.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Asn1.Ocsp;
using Org.BouncyCastle.Crypto.Tls;
using System;
using System.Net;
using System.IO;
using System.Drawing.Drawing2D;
using System.Security.Policy;

namespace BE.Services.RulesServices
{
	public class RulesService : IRulesService
	{
		private readonly AppDbContext _db;
		private readonly IMapper _mapper;

		public RulesService(AppDbContext db, IMapper mapper)
		{
			_db = db;
			_mapper = mapper;
		}

		public async Task<BaseResponse<Rules>> CreateRules(AddOrUpdateRulesDTO addRulesDto, string uploads, string localServer)
		{
			var success = false;
			var message = "";
			try
			{
                //uploads = @"\\10.32.9.100\\010_Common\\999_Users\\Storage_IMS\\";
                var rule = _mapper.Map<Rules>(addRulesDto);
				if (addRulesDto.formFile != null)
				{
                    //rule.pathFile = FilesHelper.UploadFileAndReturnLink(addRulesDto.formFile, uploads, "/Rules/");
                    var ms = new MemoryStream();
                    await addRulesDto.formFile.CopyToAsync(ms);
					rule.fileName = addRulesDto.formFile.FileName;
					rule.fileType = addRulesDto.formFile.ContentType;
					rule.fileCode = ms.ToArray();
					//rule.pathFile = localServer;
                }
				rule.dateCreated = DateTime.Now;
				rule.userCreated = addRulesDto.idUser;
				await _db.Rules.AddAsync(rule);
				await _db.SaveChangesAsync();
                success = true;
				message = "Add new Rules successfully";
				return new BaseResponse<Rules>(success, message, rule);
			}
			catch (Exception ex)
			{
				success = false;
				message = $"Adding new Rules failed! {ex.Message}";
				return new BaseResponse<Rules>(success, message, new Rules());
			}
		}

		public async Task<BaseResponse<Rules>> UpdateRules(int id, AddOrUpdateRulesDTO updateRulesDto, string upload, string localServer)
		{
			var success = false;
			var message = "";
			var data = new Rules();
			try
			{
                //upload = localServer = @"\\10.32.9.100\\010_Common\\999_Users\\Storage_IMS\\";

                var rule = await _db.Rules.Where(s => s.id.Equals(id)).FirstOrDefaultAsync();
				if (rule is null)
				{
					message = "Rules doesn't exist !";
					data = null;
					return new BaseResponse<Rules>(success, message, data);
				}
				var ruleMapdata = _mapper.Map<AddOrUpdateRulesDTO, Rules>(updateRulesDto, rule);
				if (updateRulesDto.formFile != null)
				{
					//var fileName = rule.pathFile?.Replace($"{localServer}/Rules/", "");
					//string filePath = System.IO.Path.Combine(upload, "Rules", fileName ?? "");
					//if (File.Exists(filePath))
					//{
					//	File.Delete(filePath);
					//}
					//ruleMapdata.pathFile = FilesHelper.UploadFileAndReturnLink(updateRulesDto.formFile, upload, "/Rules/");

                    var ms = new MemoryStream();
                    await updateRulesDto.formFile.CopyToAsync(ms);
                    rule.fileName = updateRulesDto.formFile.FileName;
                    rule.fileType = updateRulesDto.formFile.ContentType;
                    rule.fileCode = ms.ToArray();
                    //rule.pathFile = localServer;
                }
				ruleMapdata.dateUpdated = DateTime.Now;
				ruleMapdata.userUpdated = updateRulesDto.idUser;
				_db.Rules.Update(ruleMapdata);
				await _db.SaveChangesAsync();
				success = true;
				message = "Editing Rules successfully";
				data = ruleMapdata;
				return new BaseResponse<Rules>(success, message, data);
			}
			catch (Exception ex)
			{
				success = false;
				message = $"Editing Rules failed! {ex.Message}";
				return new BaseResponse<Rules>(success, message, data = null);
			}
		}
		public async Task<BaseResponse<Rules>> DeleteRules(int idRules, DeleteRulesDTO deleteRules)
		{
			var success = false;
			var message = "";
			var data = new Rules();
			try
			{
				var rule = await _db.Rules.Where(s => s.id.Equals(idRules)).FirstOrDefaultAsync();
				if (rule is null)
				{
					success = false;
					message = "Rules doesn't exist !";
					return new BaseResponse<Rules>(success, message, data = null);
				}
				rule.dateDeleted = DateTime.Now;
				rule.isDeleted = true;
				rule.userDeleted = deleteRules.idUser;
				_db.Rules.Update(rule);
				await _db.SaveChangesAsync();
				success = true;
				message = "Deleting Rules successfully";
				return new BaseResponse<Rules>(success, message, rule);
			}
			catch (Exception ex)
			{
				success = false;
				message = $"Deleting Rules failed! {ex.InnerException}";
				return new BaseResponse<Rules>(success, message, data = null);
			}
		}
		public async Task<BaseResponse<List<Rules>>> GetAllRulesAsync()
		{
			var success = false;
			var message = "";
			var data = new List<Rules>();
			try
			{
				var rule = await _db.Rules
					.Where(s => s.isDeleted == false)
					.OrderByDescending(s => s.dateCreated)
					.ToListAsync();

				success = true;
				message = "Get all data successfully";
				data.AddRange(rule);
				return (new BaseResponse<List<Rules>>(success, message, data));
			}
			catch (Exception ex)
			{
				success = false;
				message = ex.Message;
				return (new BaseResponse<List<Rules>>(success, message, data));
			}
		}
		public async Task<BaseResponse<List<Rules>>> GetRulesWithUserId(int userId)
		{
			var success = false;
			var message = "";
			var data = new List<Rules>();
			try
			{
				var rule = await _db.Rules
					.OrderByDescending(s => s.dateCreated)
					.Where(s => s.isDeleted == false
							&& s.userCreated.Equals(userId))
					.ToListAsync();
				success = true;
				message = "Get data successfully";
				data.AddRange(rule);
				return (new BaseResponse<List<Rules>>(success, message, data));
			}
			catch (Exception ex)
			{
				success = false;
				message = ex.Message;
				return (new BaseResponse<List<Rules>>(success, message, data));
			}
		}
		public async Task<BaseResponse<List<Rules>>> FindRulesByTitle(string searchTitle)
		{
			var success = false;
			var message = "";
			var data = new List<Rules>();
			try
			{
				var rule = await _db.Rules.Where(s =>
						s.title.ToLower().Contains(searchTitle.ToLower().Trim())
						&& s.isDeleted == false).ToListAsync();

				success = true;
				message = "Get all data successfully";
				data.AddRange(rule);
				return (new BaseResponse<List<Rules>>(success, message, data));
			}
			catch (Exception ex)
			{
				success = false;
				message = ex.Message;
				return (new BaseResponse<List<Rules>>(success, message, data));
			}
		}

		public async Task<BaseResponse<Rules>> DownloadFile(int idRules)
		{
            var success = false;
            var message = "";
            var data = new Rules();
            try
            {
                var rule = await _db.Rules.Where(s => s.id.Equals(idRules)).FirstOrDefaultAsync();
                if (rule is null)
                {
                    success = false;
                    message = "Download file doesn't exist !";
                    return new BaseResponse<Rules>(success, message, data);
                }
				//string fileName = rule.pathFile.Substring(rule.pathFile.LastIndexOf("/")+1);
    //            string downloadFolderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads");
    //            string downloadFilePath = Path.Combine(downloadFolderPath, fileName);
    //            var client = new WebClient();
				//client.DownloadFile(rule.pathFile, downloadFilePath);

				
                success = true;
                message = "Download file successfully";
                return new BaseResponse<Rules>(success, message, rule);
            }
            catch (Exception ex)
            {
                success = false;
                message = $"Download file failed! {ex.InnerException}";
                return new BaseResponse<Rules>(success, message, data);
            }
        }
		

	}
}
