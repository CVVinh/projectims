using AutoMapper;
using BE.Data.Contexts;
using BE.Data.Dtos.PaidDtos;
using BE.Data.Models;
using BE.Helpers;
using BE.Response;
using DocumentFormat.OpenXml.InkML;
using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.EntityFrameworkCore;
using NuGet.Packaging;
using System.Collections.Generic;
using System.Globalization;
using System.Linq.Expressions;
using static BE.Data.Enum.LeaveOff.Status;
using System.Globalization;
using System;

namespace BE.Services.PaidServices
{
    public interface IPaidServices
    {
        Task<BaseResponse<List<Paids>>> GetAllPaidAsync();
        Task<BaseResponse<Paids>> GetPaidWithIdAsync(int Id);
        Task<BaseResponse<List<Paids>>> GetPaidWithUserIdAsync(int userId);
        Task<BaseResponse<List<Paids>>> GetPaidWithSampleIdAsync(int sampleId);
        Task<BaseResponse<List<Paids>>> SearchPaidByDayAsync(SearchDayPaidDtos searchDayPaidDtos);
        Task<BaseResponse<Paids>> CreatePaid(CreatePaidDtos createPaidDtos, string root, string rootPath);
        Task<BaseResponse<Paids>> DeletePaid(int idPaid, string root, string local);
        Task<BaseResponse<Paids>> EditPaid(int id, CreatePaidDtos createPaidDtos, string root, string local);
        Task<BaseResponse<List<PaidImage>>> DeleteMutilImgPaid(int idPaid, List<int>? listIdImg, string root, string local);
        Task<BaseResponse<Paids>> AccepterPayment(int idPaid, AcceptPaymentPaidDtos acceptPaymentPaidDtos);
        Task<BaseResponse<Paids>> NotAccepterPayment(int idPaid, AcceptPaymentPaidDtos acceptPaymentPaidDtos);

        Task<BaseResponse<List<PaidResponseDtos>>> GetAllPaid();
        Task<BaseResponse<PaidResponseDtos>> GetPaidWithId(int Id);
        Task<BaseResponse<List<PaidResponseDtos>>> GetPaidWithUserId(int userId);
        Task<BaseResponse<List<PaidResponseDtos>>> GetPaidWithSampleId(int sampleId);
        Task<BaseResponse<List<PaidResponseDtos>>> SearchPaidByDay(SearchDayPaidDtos searchDayPaidDtos);
    }
    public class PaidService : IPaidServices
    {
        private readonly AppDbContext _appContext;
        private readonly IMapper _mapper;
        public PaidService(AppDbContext appContext, IMapper mapper)
        {
            _appContext = appContext;
            _mapper = mapper;
        }
        public async Task<BaseResponse<List<Paids>>> GetAllPaidAsync()
        {
            var success = false;
            var message = "";
            var data = new List<Paids>();
            try
            {
                var paids = await _appContext.Paids.Include(x => x.paidImages).ToListAsync();
                success = true;
                message = "Get all data successfully";
                data.AddRange(paids);
                return (new BaseResponse<List<Paids>>(success, message, data));
            }
            catch (Exception ex)
            {
                success = true;
                message = ex.Message;
                return (new BaseResponse<List<Paids>>(success, message, data));
            }
        }

        private List<PaidResponseDtos> getData()
        {
            NumberFormatInfo nfi = new CultureInfo("vi-VN", false).NumberFormat;
            nfi.CurrencyPositivePattern = 3;

            var getPaids = (from p in _appContext.Paids
                                 join up in _appContext.Users on p.PaidPerson equals up.id
                                 join cs in _appContext.Customers on Convert.ToInt32(p.CustomerName) equals cs.id
                                 join pr in _appContext.PaidReasons on Convert.ToInt32(p.PaidReason) equals pr.id
                                 join pj in _appContext.Projects on p.ProjectId equals pj.Id into pj1
                                 from pj in pj1.DefaultIfEmpty()
                                 join uc in _appContext.Users on p.PersonConfirm equals uc.id into uc1
                                 from uc in uc1.DefaultIfEmpty()
                                 select new PaidResponseDtos
                                 {
                                     id = p.Id,
                                     paidPerson = p.PaidPerson,
                                     paidReasonName = pr.name,

                                     personConfirm = Convert.ToInt32(p.PersonConfirm),
                                     personConfirmName = uc != null ? uc.FullName : "",

                                     paidDate = p.PaidDate != DateTime.MinValue ? p.PaidDate.ToString("MM/dd/yyyy") : null,
                                     createDate = p.CreateDate != DateTime.MinValue ? p.CreateDate.ToString("MM/dd/yyyy") : null,

                                     projectId = p.ProjectId,
                                     nameProject = pj != null ? pj.Name : "",
                                     isDelPro = pj != null ? pj.IsDeleted : false,

                                     customerName = Convert.ToInt32(p.CustomerName),
                                     customerFullName = cs.fullName,

                                     amountPaid = p.AmountPaid,
                                     //amountPaidName = p.AmountPaid.ToString("C", CultureInfo.CreateSpecificCulture("vi-VN")),
                                     amountPaidName = string.Format(nfi, "{0:C}", p.AmountPaid),

                                     paidReason = Convert.ToInt32(p.PaidReason),
                                     paidPersonName = up.FullName,

                                     contentReason = p.ContentReason,
                                     reasonRefusal = p.ReasonRefusal,
                                     isAccept = p.IsAccept,
                                     paidImages = p.paidImages,
                                 }).OrderBy(s => s.isAccept).ToList();
            
            return getPaids;
        }

        // ================================================
        public async Task<BaseResponse<List<PaidResponseDtos>>> GetAllPaid()
        {
            var success = false;
            var message = "";
            var data = new List<PaidResponseDtos>();
            try
            {
                var getPaids = getData();
                success = true;
                message = "Get all data successfully";
                return (new BaseResponse<List<PaidResponseDtos>>(success, message, getPaids));
            }
            catch (Exception ex)
            {
                success = true;
                message = ex.Message;
                return (new BaseResponse<List<PaidResponseDtos>>(success, message, data));
            }
        }

        public async Task<BaseResponse<PaidResponseDtos>> GetPaidWithId(int Id)
        {
            var success = false;
            var message = "";
            var data = new PaidResponseDtos();
            try
            {
                var getPaids = getData().Where(p => p.id == Id).FirstOrDefault();
                if (getPaids is null)
                {
                    message = "paid doesn't exist !";
                    return new BaseResponse<PaidResponseDtos>(success, message, data);
                }
                success = true;
                message = "Get all data successfully";
                return (new BaseResponse<PaidResponseDtos>(success, message, getPaids));
            }
            catch (Exception ex)
            {
                success = true;
                message = ex.Message;
                return (new BaseResponse<PaidResponseDtos>(success, message, data));
            }
        }

        public async Task<BaseResponse<List<PaidResponseDtos>>> GetPaidWithUserId(int userId)
        {
            var success = false;
            var message = "";
            var data = new List<PaidResponseDtos>();
            try
            {
                var getPaids = getData().Where(p => p.paidPerson == userId).ToList();
                success = true;
                message = "Get all data successfully";
                return (new BaseResponse<List<PaidResponseDtos>>(success, message, getPaids));
            }
            catch (Exception ex)
            {
                success = true;
                message = ex.Message;
                return (new BaseResponse<List<PaidResponseDtos>>(success, message, data));
            }
        }
        public async Task<BaseResponse<List<PaidResponseDtos>>> GetPaidWithSampleId(int sampleId)
        {
            var success = false;
            var message = "";
            var data = new List<PaidResponseDtos>();
            try
            {
                var getPaids = getData();
                var listpaids = (from x in getPaids
                                join c in _appContext.Users on x.paidPerson equals c.id
                                join us in _appContext.UserGroups on c.id equals us.idUser
                                where us.idGroup != 2 || x.paidPerson == sampleId
                                select x).ToList();

                success = true;
                message = "Get all data successfully";
                return (new BaseResponse<List<PaidResponseDtos>>(success, message, listpaids));
            }
            catch (Exception ex)
            {
                success = true;
                message = ex.Message;
                return (new BaseResponse<List<PaidResponseDtos>>(success, message, data));
            }
        }
        public async Task<BaseResponse<List<PaidResponseDtos>>> SearchPaidByDay(SearchDayPaidDtos searchDayPaidDtos)
        {
            var success = false;
            var message = "";
            var data = new List<PaidResponseDtos>();

            if (searchDayPaidDtos.startDate == null)
            {
                success = false;
                message = "Getting paid failed! Input startDate argument are not provided!";
                return (new BaseResponse<List<PaidResponseDtos>>(success, message, data));
            }

            if (searchDayPaidDtos.endDate != null && searchDayPaidDtos.startDate > searchDayPaidDtos.endDate)
            {
                success = false;
                message = "startDate is not greater than endDate!";
                return (new BaseResponse<List<PaidResponseDtos>>(success, message, data));
            }
            try
            {
                var getPaidByDay = new List<PaidResponseDtos>();
                var getPaids = getData();

                if (searchDayPaidDtos.id <= 0)
                {
                    if (searchDayPaidDtos.endDate != null)
                    {
                        getPaidByDay = getPaids.Where(x => Convert.ToDateTime(x.paidDate!=null ? x.paidDate: DateTime.MinValue) >= searchDayPaidDtos.startDate && Convert.ToDateTime(x.paidDate != null ? x.paidDate : DateTime.MinValue) <= searchDayPaidDtos.endDate).ToList();
                    }
                    else
                    {
                        getPaidByDay = getPaids.Where(x => Convert.ToDateTime(x.paidDate != null ? x.paidDate : DateTime.MinValue) >= searchDayPaidDtos.startDate).ToList();
                    }
                }
                else
                {
                    if (searchDayPaidDtos.endDate != null)
                    {
                        getPaidByDay = getPaids.Where(x => Convert.ToDateTime(x.paidDate != null ? x.paidDate : DateTime.MinValue) >= searchDayPaidDtos.startDate && Convert.ToDateTime(x.paidDate != null ? x.paidDate : DateTime.MinValue) <= searchDayPaidDtos.endDate && x.paidPerson == searchDayPaidDtos.id).ToList();
                    }
                    else
                    {
                        getPaidByDay = getPaids.Where(x => Convert.ToDateTime(x.paidDate != null ? x.paidDate : DateTime.MinValue) >= searchDayPaidDtos.startDate && x.paidPerson == searchDayPaidDtos.id).ToList();
                    }
                }
                success = true;
                message = "Get all data successfully";
                data = getPaidByDay;
                return (new BaseResponse<List<PaidResponseDtos>>(success, message, data));
            }
            catch (Exception ex)
            {
                success = false;
                message = "Getting paid failed! An error occurred: " + ex.Message;
                return (new BaseResponse<List<PaidResponseDtos>>(success, message, data));
            }
        }
        // ================================================

        public async Task<BaseResponse<Paids>> GetPaidWithIdAsync(int Id)
        {
            var success = false;
            var message = "";
            var data = new Paids();
            try
            {
                var paids = await _appContext.Paids.Where(x => x.Id == Id).Include(x => x.paidImages).FirstOrDefaultAsync();
                if (paids is null)
                {
                    message = "paid doesn't exist !";
                    return new BaseResponse<Paids>(success, message, data);
                }
                success = true;
                message = "Get all data successfully";
                return (new BaseResponse<Paids>(success, message, paids));
            }
            catch (Exception ex)
            {
                success = true;
                message = ex.Message;
                return (new BaseResponse<Paids>(success, message, data));
            }
        }

        public async Task<BaseResponse<List<Paids>>> GetPaidWithUserIdAsync(int userId)
        {
            var success = false;
            var message = "";
            var data = new List<Paids>();
            try
            {
                var paids = await _appContext.Paids.Where(x => x.PaidPerson == userId).Include(x => x.paidImages).ToListAsync();
                success = true;
                message = "Get all data successfully";
                return (new BaseResponse<List<Paids>>(success, message, paids));
            }
            catch (Exception ex)
            {
                success = true;
                message = ex.Message;
                return (new BaseResponse<List<Paids>>(success, message, data));
            }
        }

        public async Task<BaseResponse<List<Paids>>> GetPaidWithSampleIdAsync(int sampleId)
        {
            var success = false;
            var message = "";
            var data = new List<Paids>();
            try
            {
                var listpaid = await (from x in _appContext.Paids
                                      join c in _appContext.Users on x.PaidPerson equals c.id
                                      join us in _appContext.UserGroups on  c.id equals us.idUser
                                      where us.idGroup != 2 || x.PaidPerson == sampleId
                                      select x).Include(x => x.paidImages).ToListAsync();

                success = true;
                message = "Get all data successfully";
                return (new BaseResponse<List<Paids>>(success, message, listpaid));
            }
            catch (Exception ex)
            {
                success = true;
                message = ex.Message;
                return (new BaseResponse<List<Paids>>(success, message, data));
            }
        }

        public async Task<BaseResponse<Paids>> CreatePaid(CreatePaidDtos createPaidDtos, string root, string local)
        {
            var success = false;
            var message = "";
            var data = new Paids();
            try
            {
                if (createPaidDtos == null)
                {
                    throw new ArgumentNullException(nameof(createPaidDtos), "createPaidDtos cannot be null");
                }

                var paid = _mapper.Map<Paids>(createPaidDtos);
                if (createPaidDtos.paidImage != null)
                {
                    foreach (var item in createPaidDtos.paidImage)
                    {
                        paid.paidImages = paid.paidImages ?? new List<PaidImage>();
                        var ms = new MemoryStream();
                        await item.CopyToAsync(ms);

                        var base64String = Convert.ToBase64String(ms.ToArray());
                        var imgSrc = $"data:{item.ContentType};base64,{base64String}";

                        paid.paidImages.Add(new PaidImage()
                        {
                            ImageName = item.FileName,
                            ImageType = item.ContentType,
                            ImageCode = ms.ToArray(),
                            ImagePath = imgSrc,
                        });
                    }
                }
                else
                {
                    paid.paidImages = new List<PaidImage>();
                }
                if (paid != null)
                {
                    paid.IsAccept = 0;
                    await _appContext.Paids.AddAsync(paid);
                    await _appContext.SaveChangesAsync();

                    success = true;
                    message = "Add new Paid successfully";
                    data = paid;
                }

                return new BaseResponse<Paids>(success, message, data);
            }
            catch (Exception ex)
            {
                message = $"Adding new Paid failed! {ex}";
                data = null;
                return new BaseResponse<Paids>(success, message, data);
            }
        }

        public async Task<BaseResponse<List<Paids>>> SearchPaidByDayAsync(SearchDayPaidDtos searchDayPaidDtos)
        {
            var success = false;
            var message = "";
            var data = new List<Paids>();

            if (searchDayPaidDtos.startDate == null)
            {
                success = false;
                message = "Getting paid failed! Input startDate argument are not provided!";
                return (new BaseResponse<List<Paids>>(success, message, data));
            }

            if (searchDayPaidDtos.endDate != null && searchDayPaidDtos.startDate > searchDayPaidDtos.endDate)
            {
                success = false;
                message = "startDate is not greater than endDate!";
                return (new BaseResponse<List<Paids>>(success, message, data));
            }
            try
            {
                var getPaidByDay = new List<Paids>();
                if (searchDayPaidDtos.id <= 0)
                {
                    if (searchDayPaidDtos.endDate != null)
                    {
                        getPaidByDay = await _appContext.Paids.Where(x => x.PaidDate.Date >= searchDayPaidDtos.startDate && x.PaidDate.Date <= searchDayPaidDtos.endDate).Include(x => x.paidImages).ToListAsync();
                    }
                    else
                    {
                        getPaidByDay = await _appContext.Paids.Where(x => x.PaidDate.Date >= searchDayPaidDtos.startDate).Include(x => x.paidImages).ToListAsync();
                    }
                }
                else
                {
                    if (searchDayPaidDtos.endDate != null)
                    {
                        getPaidByDay = await _appContext.Paids.Where(x => x.PaidDate.Date >= searchDayPaidDtos.startDate && x.PaidDate.Date <= searchDayPaidDtos.endDate && x.PaidPerson == searchDayPaidDtos.id).Include(x => x.paidImages).ToListAsync();
                    }
                    else
                    {
                        getPaidByDay = await _appContext.Paids.Where(x => x.PaidDate.Date >= searchDayPaidDtos.startDate && x.PaidPerson == searchDayPaidDtos.id).Include(x => x.paidImages).ToListAsync();
                    }
                }
                success = true;
                message = "Get all data successfully";
                data = getPaidByDay;
                return (new BaseResponse<List<Paids>>(success, message, data));
            }
            catch (Exception ex)
            {
                success = false;
                message = "Getting paid failed! An error occurred: " + ex.Message;
                return (new BaseResponse<List<Paids>>(success, message, data));
            }
        }


        public async Task<BaseResponse<Paids>> DeletePaid(int idPaid, string root, string local)
        {
            var success = false;
            var message = "";
            var data = new Paids();
            try
            {
                var paid = await _appContext.Paids.Where(p => p.Id == idPaid).Include(x => x.paidImages).FirstOrDefaultAsync();
                if (paid is null)
                {
                    message = "idPaid doesn't exist !";
                    return new BaseResponse<Paids>(success, message, data);
                }

                _appContext.Paids.Remove(paid);

                //if (paid.paidImages.Count > 0)
                //{
                //    foreach (var item in paid.paidImages)
                //    {
                //        var fileName = item.ImagePath?.Replace($"{local}/PaidPicture/", "");
                //        string filePath = System.IO.Path.Combine(root, "PaidPicture", fileName ?? "");
                //        if (File.Exists(filePath))
                //        {
                //            File.Delete(filePath);
                //        }
                //        _appContext.PaidImages.Remove(item);
                //    }
                //}

                if (paid.paidImages.Count > 0)
                {
                    foreach (var item in paid.paidImages)
                    {
                        _appContext.PaidImages.Remove(item);
                    }
                }

                await _appContext.SaveChangesAsync();
                success = true;
                message = "Deleting paid successfully";
                data = paid;
                return new BaseResponse<Paids>(success, message, data);
            }
            catch (Exception ex)
            {
                message = $"Deleting paid failed! {ex.InnerException}";
                data = null;
                return new BaseResponse<Paids>(success, message, data);
            }
        }


        public async Task<BaseResponse<List<PaidImage>>> DeleteMutilImgPaid(int idPaid, List<int>? listIdImg, string root, string local)
        {
            var success = false;
            var message = "";
            var data = new List<PaidImage>();
            try
            {
                var paid = await _appContext.PaidImages.Where(p => p.PaidId == idPaid).FirstOrDefaultAsync();
                if (paid is null)
                {
                    message = "idPaid doesn't exist !";
                    data = null;
                    return new BaseResponse<List<PaidImage>>(success, message, data);
                }

                foreach (var item in listIdImg)
                {
                    var itemImg = await _appContext.PaidImages.Where(p => p.PaidId == idPaid && p.ImageId == item).FirstOrDefaultAsync();
                    if (itemImg != null)
                    {
                        //var fileName = itemImg.ImagePath?.Replace($"{local}/PaidPicture/", "");
                        //string filePath = System.IO.Path.Combine(root, "PaidPicture", fileName ?? "");
                        //if (File.Exists(filePath))
                        //{
                        //    File.Delete(filePath);
                        //}
                        //_appContext.PaidImages.Remove(itemImg);
                        //data.Add(itemImg);

                        _appContext.PaidImages.Remove(itemImg);
                        data.Add(itemImg);
                    }
                    else if (itemImg == null)
                    {
                        message += "ImageId " + item + ", ";
                    }
                }
                await _appContext.SaveChangesAsync();

                success = true;
                if (message == "")
                {
                    message = "Deleting list image successfully";
                }
                else
                {
                    message += "delete error!";
                }
                return new BaseResponse<List<PaidImage>>(success, message, data);
            }
            catch (Exception ex)
            {
                message = $"Deleting paid failed! {ex.InnerException}";
                data = null;
                return new BaseResponse<List<PaidImage>>(success, message, data);
            }
        }

        public async Task<BaseResponse<Paids>> EditPaid(int id, CreatePaidDtos createPaidDtos, string root, string local)
        {
            var success = false;
            var message = "";
            var data = new Paids();
            try
            {
                var paid = await _appContext.Paids.Where(p => p.Id == id).Include(x => x.paidImages).FirstOrDefaultAsync();
                if (paid is null)
                {
                    message = "Paid doesn't exist !";
                    data = null;
                    return new BaseResponse<Paids>(success, message, data);
                }

                if (createPaidDtos.paidImage != null)
                {
                    //foreach(var item in paid.paidImages)
                    //{
                    //    var fileName = item.ImagePath?.Replace($"{local}/PaidPicture/", "");
                    //    string filePath = System.IO.Path.Combine(root, "PaidPicture", fileName ?? "");
                    //    if (File.Exists(filePath))
                    //    {
                    //        File.Delete(filePath);
                    //        _appContext.PaidImages.Remove(item);
                    //    }
                    //}

                    foreach (var item in createPaidDtos.paidImage)
                    {
                        paid.paidImages = paid.paidImages ?? new List<PaidImage>();
                        var ms = new MemoryStream();
                        await item.CopyToAsync(ms);

                        var base64String = Convert.ToBase64String(ms.ToArray());
                        var imgSrc = $"data:{item.ContentType};base64,{base64String}";

                        paid.paidImages.Add(new PaidImage()
                        {
                            ImageName = item.FileName,
                            ImageType = item.ContentType,
                            ImageCode = ms.ToArray(),
                            ImagePath = imgSrc,
                        });
                    }
                }

                var paidMap = _mapper.Map<CreatePaidDtos, Paids>(createPaidDtos, paid);
                _appContext.Paids.Update(paidMap);
                await _appContext.SaveChangesAsync();

                success = true;
                message = "Editing Paid successfully";
                data = paidMap;
                return new BaseResponse<Paids>(success, message, data);
            }
            catch (Exception ex)
            {
                message = $"Editing leave off failed! {ex.Message}";
                data = null;
                return new BaseResponse<Paids>(success, message, data);
            }
        }

        public async Task<BaseResponse<Paids>> AccepterPayment(int idPaid, AcceptPaymentPaidDtos acceptPaymentPaidDtos)
        {
            var success = false;
            var message = "";
            var data = new Paids();
            try
            {
                var paid = await _appContext.Paids.Where(x => x.Id == idPaid && x.IsAccept == 0).FirstOrDefaultAsync();
                if (paid is null)
                {
                    message = "idPaid doesn't exist or payment!";
                    data = null;
                    return new BaseResponse<Paids>(success, message, data);
                }

                paid.IsAccept = 1;
                paid.PersonConfirm = acceptPaymentPaidDtos.PersonConfirm;
                paid.PaidDate = DateTime.Now;
                _appContext.Paids.Update(paid);
                await _appContext.SaveChangesAsync();

                success = true;
                message = "Accepting payment successfully";
                data = paid;
                return new BaseResponse<Paids>(success, message, data);
            }
            catch (Exception ex)
            {
                message = $"Accepting leave off failed! {ex.InnerException}";
                data = null;
                return new BaseResponse<Paids>(success, message, data);
            }
        }

        public async Task<BaseResponse<Paids>> NotAccepterPayment(int idPaid, AcceptPaymentPaidDtos acceptPaymentPaidDtos)
        {
            var success = false;
            var message = "";
            var data = new Paids();
            try
            {
                var paid = await _appContext.Paids.Where(x => x.Id == idPaid && x.IsAccept == 0).FirstOrDefaultAsync();
                if (paid is null)
                {
                    message = "idPaid doesn't exist or payment!";
                    data = null;
                    return new BaseResponse<Paids>(success, message, data);
                }

                var paidMap = _mapper.Map<AcceptPaymentPaidDtos, Paids>(acceptPaymentPaidDtos, paid);
                paid.IsAccept = 2;
                paid.PaidDate = DateTime.Now;
                _appContext.Paids.Update(paid);
                await _appContext.SaveChangesAsync();

                success = true;
                message = "Accepting payment successfully";
                data = paid;
                return new BaseResponse<Paids>(success, message, data);
            }
            catch (Exception ex)
            {
                message = $"Accepting leave off failed! {ex.InnerException}";
                data = null;
                return new BaseResponse<Paids>(success, message, data);
            }
        }



    }
}
