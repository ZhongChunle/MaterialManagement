using LibraryManagementSystem.Entity;
using MaterialManagement.Entity;
using MaterialManagement.Models;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
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
                //分页
                // var list_Subject_set = llist.Take(PageSize*PageIndex).Skip(PageSize* (PageIndex- 1)).ToList();
                pageResult.rows = list.Take(queryData.pageSize * queryData.currentPage).Skip(queryData.pageSize * (queryData.currentPage - 1)).ToList();
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
            int renice = 0; // 上传文件已存在数据条数
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

                        foreach (DataRow row in dtexcel.Rows)
                        {
                            // 创建一个对象存储上传的数据，并循环添加到一个List结合中
                            selectData selects = new selectData();
                            try
                            {
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

                        // 遍历上传文件的重复数据
                        List<selectData> selectData2 = new List<selectData>();
                        
                        for (int i = 0; i < selectData.Count; i++)
                        {
                            if (i != 0)
                            {
                                // 倒序删除的方法，否则删除回出现报错
                                // 每一次删除都会导致集合的大小和元素索引值发生变化，从而导致在foreach中删除元素会出现异常。
                                for (int j = selectData2.Count - 1; j >= 0; j--) {
                                    if (selectData[i].Code.Trim() == selectData2[j].Code.Trim())
                                    {
                                        renice++;
                                        selectData2.Remove(selectData2[j]);
                                    }
                                }
                                selectData2.Add(selectData[i]);
                            }
                            else {
                                selectData2.Add(selectData[0]);
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
                                result.message = "成功读取数据："+ selectData.Count+ "条，数据库比对重复：" + oldCount + "条，上传数据自身重复："+ renice + "条,成功上传：" + (ServiceStationCount - renice) + "条数据";
                                queryData query = new queryData();
                                query.currentPage = 1;
                                query.pageSize = 5;
                                Session["ExcelFile"] = selectData2; // 赋值给本地保存
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

        #region 八、确认上传文件事件
        public ActionResult ConfirmUpload() { 
            Result result = new Result();
            int success = 0; // 成功的条数
            int error = 0; // 失败的条数
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
                // 将上传的数据批量导入到数据库中
                if (listSecond != null)
                {
                    // 遍历每一项集合获取里面的4个值获取对应的id
                    foreach (var item in listSecond)
                    {
                        try
                        {
                            //事务开始(记得结束)
                            using (var scope = new TransactionScope())
                            {
                                // 经销商
                                var franchiser_id = myModels.franchiser_table.Where(o => o.franchiser_name == item.franchiser).Single().franchiser_id;
                                // 生产商
                                var manufacturer_id = myModels.manufacturer_table.Where(o => o.manufacturer_name == item.producer).Single().manufacturer_id;
                                // 生产类型
                                var product_type_id = myModels.product_type.Where(o => o.product_type_name == item.productType).Single().product_type_id;
                                // 中标类型
                                var the_winning_type_id = myModels.the_winning_type.Where(o => o.the_winning_type_name == item.TheWinningType).Single().the_winning_type_id;
                                // 创建物资标对象添加数据
                                matter_table matter = new matter_table();
                                matter.matter_name = item.Name;
                                matter.matter_code = item.Code;
                                matter.purchase_price = item.purchasePrice;
                                matter.Selling_price = item.SellingPrice;
                                matter.manufacturer_id = manufacturer_id;
                                matter.franchiser_id = franchiser_id;
                                matter.product_type_id = product_type_id;
                                matter.the_winning_type_id = the_winning_type_id;
                                // 调用添加数据的方法
                                myModels.matter_table.Add(matter);
                                myModels.SaveChanges();
                                success++;
                                // 提交事务
                                scope.Complete();
                            }
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e);
                            error++;
                        }
                    }
                }
                if (success > 0 && error == 0)
                {
                    result.flag = true;
                    result.message = "导入数据添加成功：" + success + "条";
                }
                else {
                    result.flag = false;
                    result.message = "导入数据添加成功：" + success + "条，失败：" + error + "条";
                }
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                result.flag = false;
                result.message = "上传文件保存数据库异常："+e.Message;
                return Json(result, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        #region NPOI导出到Excel 
        //NPOI导出到Excel(故障码)
        public ActionResult ExportToExcel(queryData queryData,Boolean flag)
        {
            // int FaultCodeID = 0;//故障码ID

            //查询数据
            // var listFaultInfo = listFaultCodePush(FaultCodeID, DTC);
            // List<PlatformClass> listExaminee = listFaultInfo;
            try
            {
                List<selectData> listFaultInfo = (from tba in myModels.matter_table
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
                if (flag == true) {
                    // 进行模糊查询
                    if (queryData.name != null && queryData.name.Length > 0)
                    {
                        listFaultInfo = listFaultInfo.Where(o => o.Name.Contains(queryData.name)).ToList();
                    }
                    if (queryData.code != null && queryData.code.Length > 0)
                    {
                        listFaultInfo = listFaultInfo.Where(o => o.Code.Contains(queryData.code)).ToList();
                    }
                    listFaultInfo = listFaultInfo.Take(queryData.pageSize * queryData.currentPage).Skip(queryData.pageSize * (queryData.currentPage - 1)).ToList();
                }
                //二：代码创建一个Excel表格（这里称为工作簿）
                //创建Excel文件的对象 工作簿(调用NPOI文件)
                HSSFWorkbook excelBook = new HSSFWorkbook();
                ICellStyle style1 = excelBook.CreateCellStyle();//声明style1对象，设置Excel表格的样式
                ICellStyle style2 = excelBook.CreateCellStyle();
                ICellStyle style3 = excelBook.CreateCellStyle();
                IFont font = excelBook.CreateFont();
                font.Color = IndexedColors.RED.Index;
                style3.SetFont(font);
                style1.Alignment = HorizontalAlignment.JUSTIFY;//两端自动对齐（自动换行）
                style1.VerticalAlignment = VerticalAlignment.CENTER;
                style2.Alignment = HorizontalAlignment.CENTER;
                style2.VerticalAlignment = VerticalAlignment.CENTER;
                style3.Alignment = HorizontalAlignment.CENTER;
                style3.VerticalAlignment = VerticalAlignment.CENTER;
                //创建Excel工作表 Sheet=故障码信息
                ISheet sheet1 = excelBook.CreateSheet("物资信息");
                // 第一行表格表题
                IRow row0 = sheet1.CreateRow(0);

                //给Sheet添加第二行的头部标题
                IRow row1 = sheet1.CreateRow(1);
                //给标题的每一个单元格赋值
                row1.CreateCell(0).SetCellValue("物资名称");//0
                row1.CreateCell(1).SetCellValue("物资编号");//1
                row1.CreateCell(2).SetCellValue("采购价格");//2
                row1.CreateCell(3).SetCellValue("售卖价格");//3
                row1.CreateCell(4).SetCellValue("生产商名称");//4
                row1.CreateCell(5).SetCellValue("经销商名称");//5
                row1.CreateCell(6).SetCellValue("产品类型");//6
                row1.CreateCell(7).SetCellValue("中标类型");//7
                row1.GetCell(0).CellStyle = style2;//初始化设置样式
                row1.GetCell(1).CellStyle = style2;//初始化设置样式
                row1.GetCell(2).CellStyle = style2;//初始化设置样式
                row1.GetCell(3).CellStyle = style2;//初始化设置样式
                row1.GetCell(4).CellStyle = style2;//初始化设置样式
                row1.GetCell(5).CellStyle = style2;//初始化设置样式
                row1.GetCell(6).CellStyle = style2;//初始化设置样式
                row1.GetCell(7).CellStyle = style2;//初始化设置样式
                sheet1.SetColumnWidth(0, 10 * 256);//初始化设置宽度
                sheet1.SetColumnWidth(0, 10 * 256);//初始化设置宽度
                sheet1.SetColumnWidth(0, 10 * 256);//初始化设置宽度
                sheet1.SetColumnWidth(0, 10 * 256);//初始化设置宽度
                sheet1.SetColumnWidth(1, 10 * 256);//初始化设置宽度
                sheet1.SetColumnWidth(0, 10 * 256);//初始化设置样式
                sheet1.SetColumnWidth(2, 10 * 256);//初始化设置宽度
                sheet1.SetColumnWidth(3, 10 * 256);//初始化设置宽度
                sheet1.SetColumnWidth(4, 10 * 256);//初始化设置宽度
                sheet1.SetColumnWidth(5, 10 * 256);//初始化设置宽度
                sheet1.SetColumnWidth(6, 10 * 256);//初始化设置宽度
                sheet1.SetColumnWidth(7, 10 * 256);//初始化设置宽度
                //添加数据行：将表格数据逐步写入sheet1各个行中（也就是给每一个单元格赋值）
                for (int i = 0; i < listFaultInfo.Count; i++)
                {
                    //sheet1.CreateRow(i).
                    //创建行
                    IRow rowTemp = sheet1.CreateRow(i);
                    rowTemp.Height = 62 * 15;
                    //物资名称
                    rowTemp.CreateCell(0).SetCellValue(listFaultInfo[i].Name);
                    //故障码(hex)
                    rowTemp.CreateCell(1).SetCellValue(listFaultInfo[i].Code);
                    //故障码英文描述
                    rowTemp.CreateCell(2).SetCellValue((double)listFaultInfo[i].purchasePrice);
                    //故障码中文描述
                    rowTemp.CreateCell(3).SetCellValue((double)listFaultInfo[i].SellingPrice);
                    ///故障码运行条件
                    rowTemp.CreateCell(4).SetCellValue(listFaultInfo[i].producer);
                    //故障码设置条件
                    rowTemp.CreateCell(5).SetCellValue(listFaultInfo[i].franchiser);
                    //故障码设置时发生的操作
                    rowTemp.CreateCell(6).SetCellValue(listFaultInfo[i].productType);
                    //故障恢复条件
                    rowTemp.CreateCell(7).SetCellValue(listFaultInfo[i].TheWinningType);
                    rowTemp.GetCell(0).CellStyle = style3;
                    rowTemp.GetCell(1).CellStyle = style3;
                    rowTemp.GetCell(2).CellStyle = style2;
                    rowTemp.GetCell(3).CellStyle = style2;
                    rowTemp.GetCell(4).CellStyle = style1;
                    rowTemp.GetCell(5).CellStyle = style1;
                    rowTemp.GetCell(6).CellStyle = style1;
                    rowTemp.GetCell(7).CellStyle = style1;
                }
                //输出的文件名称
                string fileName = "物资信息导出" + DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss-ffff") + ".xls";
                //把Excel转为流，输出
                //创建文件流
                System.IO.MemoryStream bookStream = new System.IO.MemoryStream();
                //将工作薄写入文件流
                excelBook.Write(bookStream);

                //输出之前调用Seek（偏移量，游标位置) 把0位置指定为开始位置
                bookStream.Seek(0, System.IO.SeekOrigin.Begin);
                //Stream对象,文件类型,文件名称
                return File(bookStream, "application/vnd.ms-excel", fileName);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }
        #endregion

    }
}