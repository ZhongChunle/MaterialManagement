using LibraryManagementSystem.Entity;
using MaterialManagement.Entity;
using MaterialManagement.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Transactions;
using System.Web;
using System.Web.Mvc;

namespace MaterialManagement.Controllers
{
    public class MaterialManagementController : Controller
    {
        // GET: MaterialManagement
        Models.Material_ManagementEntities myModels = new Models.Material_ManagementEntities();

        /// <summary>
        /// 物质管理视图
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            return View();
        }

        #region 一、查询四项下拉框方法
        /// <summary>
        /// 1、查询产品类型
        /// </summary>
        /// <returns></returns>
        public ActionResult productType() {
            var list = (from tbNid in myModels.product_type
                        select new
                        {
                            id = tbNid.product_type_id,
                            val = tbNid.product_type_name
                        }).ToList();
            return Json(list,JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 2、查询中标类型
        /// </summary>
        /// <returns></returns>
        public ActionResult TheWinningType()
        {
            var list = (from tbNid in myModels.the_winning_type
                        select new
                        {
                            id = tbNid.the_winning_type_id,
                            val = tbNid.the_winning_type_name
                        }).ToList();
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 3、查询生产厂商
        /// </summary>
        /// <returns></returns>
        public ActionResult manufacturer() {
            var list = (from tbNid in myModels.manufacturer_table
                        select new
                        {
                            id = tbNid.manufacturer_id,
                            val = tbNid.manufacturer_name
                        }).ToList();
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 4、查询经销商
        /// </summary>
        /// <returns></returns>
        public ActionResult franchiser() {
            var list = (from tbNid in myModels.franchiser_table
                        select new
                        {
                            id = tbNid.franchiser_id,
                            val = tbNid.franchiser_name
                        }).ToList();
            return Json(list, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region 二、模糊分页查询物质数据
        public ActionResult qieryList(queryData queryData) {
            Result result = new Result();
            //没有记录时，count=0,(pageSize-1)/pageSize=0,即共0页
            int pageTotal = (queryData.currentPage + queryData.pageSize - 1) / queryData.pageSize;

            // 分页查询数据
            try
            {
                var list = (from tba in myModels.matter_table
                            // 连接产品类型表
                            join tbb in myModels.product_type on tba.product_type_id equals tbb.product_type_id
                            // 连接中标类型标
                            join tbc in myModels.the_winning_type on tba.the_winning_type_id equals tbc.the_winning_type_id
                            // 连接经销商表
                            join tbd in myModels.franchiser_table on tba.franchiser_id equals tbd.franchiser_id
                            // 连接生产商表
                            join tbe in myModels.manufacturer_table on tba.manufacturer_id equals tbe.manufacturer_id
                            // 排序
                            orderby tba.matterid
                            select new selectData
                            {
                                Id = tba.matterid,
                                Name = tba.matter_name,
                                Code = tba.matter_code,
                                purchasePrice = tba.purchase_price,
                                SellingPrice = tba.Selling_price,
                                producer = tbe.manufacturer_name,
                                franchiser = tbd.franchiser_name,
                                productType = tbb.product_type_name,
                                TheWinningType = tbc.the_winning_type_name
                            }).ToList();
                // 查询数据的总数
                var count = myModels.matter_table.Count();

                // 进行模糊查询
                if (queryData.name != null && queryData.name.Length > 0)
                {
                    list = list.Where(o => o.Name.Contains(queryData.name)).ToList();
                    count = list.Count();
                }
                if (queryData.code != null && queryData.code.Length > 0) {
                    list = list.Where(o => o.Code.Contains(queryData.code)).ToList();
                    count = list.Count();
                }
                // 赋值返回的分页数据
                PageResult pageResult = new PageResult();
                pageResult.total = count;
                pageResult.rows = list;
                return Json(pageResult, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                result.flag = false;
                result.message = "数据查询失败" + e.Message;
                return Json(result, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        #region 三、物资数据添加
        public ActionResult add(matter_table addAndEdit) {
            // 创建反回的对象
            Result result =  new Result();
            // 调用数据库进行添加操作
            try
            {
                // 判断数据是否重复
                var matter = myModels.matter_table.Where(o => o.matter_code == addAndEdit.matter_code).Count();
                if (matter > 0) {
                    result.flag = false;
                    result.message = "物资编号出现重复";
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
                myModels.matter_table.Add(addAndEdit);
                myModels.SaveChanges();
                result.flag = true;
                result.message = "物资数据添加成功";
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                result.flag = false;
                result.message = "添加物资信息失败："+e.Message;
                return Json(result, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        #region 四、删除物资数据
        public ActionResult remove(int Id) {
            Result result = new Result();
            // 异常捕获
            try
            {
                // 根据id查询物资信息
                matter_table matter = myModels.matter_table.Where(o => o.matterid == Id).Single();
                if (matter != null)
                {
                    // 执行删操作
                    myModels.matter_table.Remove(matter);
                    if (myModels.SaveChanges() > 0)
                    {
                        result.flag = true;
                        result.message = "物资数据删除成功";
                    }
                    else {
                        result.flag = false;
                        result.message = "物资数据删除失败";
                    }
                }
                else {
                    result.flag = false;
                    result.message = "根据id删除数据失败(后台可能没有该id数据)";
                }
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                result.flag = false;
                result.message = "根据Id删除数据异常："+e.Message;
                return Json(result,JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        #region 五、根据id修改数据
        public ActionResult findById(int? Id) {
            Result result = new Result();

            try
            {
                // 根据id查询数据
                var list = (from tba in myModels.matter_table
                             // 连接产品类型表
                            join tbb in myModels.product_type on tba.product_type_id equals tbb.product_type_id
                            // 连接中标类型标
                            join tbc in myModels.the_winning_type on tba.the_winning_type_id equals tbc.the_winning_type_id
                            // 连接经销商表
                            join tbd in myModels.franchiser_table on tba.franchiser_id equals tbd.franchiser_id
                            // 连接生产商表
                            join tbe in myModels.manufacturer_table on tba.manufacturer_id equals tbe.manufacturer_id
                            where tba.matterid == Id
                            select new EntityClass
                            {
                                matterid = tba.matterid,
                                matter_name = tba.matter_name,
                                matter_code = tba.matter_code,
                                purchase_price = tba.purchase_price,
                                Selling_price = tba.Selling_price,
                                manufacturer_id = tba.manufacturer_id,
                                manufacturer_name = tbe.manufacturer_name,
                                franchiser_id = tba.franchiser_id,
                                franchiser_name = tbd.franchiser_name,
                                product_type_id = tba.product_type_id,
                                product_type_name = tbb.product_type_name,
                                the_winning_type_id = tba.the_winning_type_id,
                                the_winning_type_name = tbc.the_winning_type_name
                            }).Single();
                if (list != null)
                {
                    // 返回查询的数据
                    result.flag = true;
                    result.message = "数据查询成功";
                    result.data = list;
                }
                else {
                    result.flag = false;
                    result.message = "根据id查询数据为空";
                }
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                result.flag = false;
                result.message = "根据id查询数据查询异常："+e.Message;
                return Json(result,JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult recompose(matter_table addAndEdit) {
            // 创建返回的数据
            Result result = new Result();
            try
            {
                // 根据id查询数据
                matter_table matter = myModels.matter_table.Where(o => o.matterid == addAndEdit.matterid).Single();
                if (matter != null)
                {
                    // 执行修改操作，判断是否重复
                    if (matter.matter_code==addAndEdit.matter_code) {
                        result.flag = false;
                        result.message = "修改的物资编号出现重复，请修改";
                    }
                    matter.matter_name = addAndEdit.matter_name;
                    matter.matter_code = addAndEdit.matter_code;
                    matter.purchase_price = addAndEdit.purchase_price;
                    matter.Selling_price = addAndEdit.Selling_price;
                    matter.franchiser_id = addAndEdit.franchiser_id;
                    matter.manufacturer_id = addAndEdit.manufacturer_id;
                    matter.product_type_id = addAndEdit.product_type_id;
                    matter.the_winning_type_id = addAndEdit.the_winning_type_id;
                    myModels.Entry(matter).State = System.Data.Entity.EntityState.Modified;
                    if (myModels.SaveChanges() > 0)
                    {
                        result.flag = true;
                        result.message = "修改的物资数据成功";
                    }
                    else {
                        result.flag = false;
                        result.message = "修改的物资保存失败";
                    }
                }
                else {
                    result.flag = false;
                    result.message = "根据id查询修改数据异常";
                }
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                result.flag = false;
                result.message = "根据id修改数据异常："+e.Message;
                return Json(result,JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        #region 六、文件上传事件
        public ActionResult upload(HttpPostedFileBase wrapper) {
            Result result = new Result();
            //using (TransactionScope scope = new TransactionScope())
            //{
            //    scope.Complete();
            //}
            int ServiceStationCount = 0;//保存成功的条数
            int oldCount = 0;//已经存在的数据条数
            try
            {
                //文件类型
                string fileExtension = Path.GetExtension(wrapper.FileName).ToLower();
                if (".xls".Equals(fileExtension) || ".xlsx".Equals(fileExtension))
                {
                    byte[] fileBytes = new byte[wrapper.ContentLength];//定义二进制数组
                    wrapper.InputStream.Read(fileBytes, 0, wrapper.ContentLength);//读取文件内容
                    if (ExcelReader.HasData(new MemoryStream(fileBytes)))
                    {  // 判断上传的excel是否为空
                       // 创建一个列表实体类，将上传的数据临时保存起来展示
                        List<selectData> selectData = new List<selectData>();
                        //获取 Excel的数据，放进DataTable中
                        DataTable dtexcel = ExcelReader.RenderFromExcel(new MemoryStream(fileBytes), 0, 1);
                        // 将第二行删除
                        dtexcel.Rows.RemoveAt(0);
                        //遍历datatable 获取数据
                        foreach (DataRow row in dtexcel.Rows)
                        {
                            // 创建一个对象存储上传的数据，并循环添加到一个List结合中
                            selectData selects = new selectData();
                            try
                            {
                                // 将物资编码作为条件查询重复数据
                                var Code = row.ItemArray[5].ToString().Trim();
                                // 循环遍历上传的数据与数据进行比对【数据比对之后再添加数据进入对象中】
                                var repetitionCount = myModels.matter_table.Where(o => o.matter_code == Code).Count();
                                // 查询重复数量
                                if (repetitionCount > 0) {
                                    oldCount++;
                                }
                                else {
                                    // 上传的excel单元格赋值实体类信息
                                    selects.producer = row.ItemArray[0].ToString().Trim();
                                    selects.franchiser = row.ItemArray[1].ToString().Trim();
                                    selects.productType = row.ItemArray[2].ToString().Trim();
                                    selects.TheWinningType = row.ItemArray[3].ToString().Trim();
                                    selects.Name = row.ItemArray[4].ToString().Trim();
                                    selects.purchasePrice = Convert.ToDecimal(row.ItemArray[6]);
                                    selects.SellingPrice = Convert.ToDecimal(row.ItemArray[7]);
                                    selects.Code = Code;
                                    // 保存数据到集合中
                                    selectData.Add(selects);
                                    ServiceStationCount++;
                                }
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine(e);
                                result.flag = false;
                                result.message = "上传文件添加集合失败：" + e.Message;
                            }
                        }
                        // Session.Abandon();//清除全部Session
                        //清除某个Session
                        Session["ExcelFile"] = null;
                        Session.Remove("ExcelFile");
                        #region 返回数据
                            // 判断读取成功的数据是否为0
                            if (ServiceStationCount == 0 && oldCount > 0)
                            {
                                result.flag = false;
                                result.message = "上传数据比对重复：" + oldCount + "条，请重新上传";
                            }
                            else {
                                // 先清空再赋值
                                result.flag = true;
                                result.message = "成功读取数据："+ selectData.Count+ "条，数据库比对重复：" + oldCount + "条,成功上传：" + ServiceStationCount + "条数据";
                                queryData query = new queryData();
                                query.currentPage = 1;
                                query.pageSize = 5;
                                Session["ExcelFile"] = selectData; // 赋值给本地保存
                                result.data = findByPage(query); // 返回分页的数据
                        }
                        #endregion
                    }
                    else
                    {
                        result.flag = false;
                        result.message = "该Excel文件为空！";
                    }
                }
                else
                {
                    result.flag = false;
                    result.message = "上传的文件类型不符";
                }
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                result.flag = false;
                result.message = "文件上传异常：" + e.Message;
                Console.WriteLine(e);
                return Json(wrapper, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        #region 七、对上传的文件进行分页查询返回页面
        public ActionResult findByPage(queryData queryData) {
            PageResult pageResult = new PageResult();
            try
            {
                //创建一个SeconBook集合对象
                List<selectData> listSecond = new List<selectData>();
                //判断Session中是否为空
                if (Session["ExcelFile"] != null)
                {
                    //获取Session中保存的数据强转为selectData集合对象
                    listSecond = Session["ExcelFile"] as List<selectData>;
                }
                //总行数
                int totalRow = listSecond.Count();
                //分页
                pageResult.total = totalRow;
                // var list_Subject_set = llist.Take(PageSize*PageIndex).Skip(PageSize* (PageIndex- 1)).ToList();
                pageResult.rows = listSecond.Take(queryData.pageSize * queryData.currentPage).Skip(queryData.pageSize * (queryData.currentPage - 1)).ToList();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            return Json(pageResult, JsonRequestBehavior.AllowGet);
        }
        #endregion
    }
}