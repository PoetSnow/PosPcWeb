using Kendo.Mvc.UI;
using Kendo.Mvc.UI.Fluent;
using System;
using System.Web.Mvc;

namespace Gemstar.BSPMS.Hotel.Web.Models
{
    public class KendoGridEditInCellHtmlHelper<T> where T : class
    {
        public KendoGridEditInCellHtmlHelper(HtmlHelper html, UrlHelper url, KendoGridCommonViewModel gridModel, Action<GridColumnFactory<T>> columnConfigurator, Action<DataSourceModelDescriptorFactory<T>> keyConfigurator, Action<GridToolBarCommandFactory<T>> customToolbarAction = null, Action<PageableBuilder> pageableBuilder = null, string controllerName = null,bool mutiple=true)
        {
            if (!string.IsNullOrWhiteSpace(controllerName))
            {
                _controllerName = controllerName;
            }
            else
            {
                _controllerName = html.ViewContext.RouteData.GetRequiredString("controller");
            }
            _html = html;
            _url = url;
            _gridModel = gridModel;
            _columnConfigurator = columnConfigurator;
            _keyConfigurator = keyConfigurator;
            _customToolbar = customToolbarAction;
            _Multiple = mutiple;
            if (pageableBuilder == null)
            {
                _pageableBuilder = pageable => pageable.PageSizes(Gemstar.BSPMS.Common.Tools.CommonHelper.PageSizes);
            }
            else
            {
                _pageableBuilder = pageableBuilder;
            }
        }

        public MvcHtmlString KendoGridEditInCell()
        {
            return new MvcHtmlString(_html.Kendo().Grid<T>()
            .Name(_gridModel.GridControlId)
            .Columns(_columnConfigurator)
            .ColumnMenu()
            .Events(e => { e.DataBound(string.IsNullOrWhiteSpace(_gridModel.onDataBound)? "onDataBound": _gridModel.onDataBound); })//加载事件可自定义
            .Editable(c => c.Mode(GridEditMode.InCell).Enabled(_gridModel.EnableFunctionForEdit))
            .Filterable()
            .Reorderable(c => c.Columns(true))
            .Resizable(c => c.Columns(true))
            .Scrollable(c => c.Height("100%").Enabled(_gridModel.EnableScrollable))
            .Sortable()
            .Selectable(s => s.Mode(GridSelectionMode.Multiple).Enabled(_Multiple))
            .Pageable(_pageableBuilder)
            .Pageable(pageable => pageable.Messages(c => c.ItemsPerPage("")))
            .HtmlAttributes(_gridModel.HtmlAttributes)
            .DataSource(dataSource => dataSource
                .Ajax()
                .Read(read => read.Action("AjaxQuery", _controllerName).Data(_gridModel.JsFuncForGetAjaxQueryPara))
                .ServerOperation(false).PageSize(_gridModel.EnableFunctionForPage ? Gemstar.BSPMS.Common.Extensions.UrlHelperExtension.GetPageSizeForCookies(System.Web.HttpContext.Current.Request) : Int32.MaxValue)
                .Batch(true)
                .Model(_keyConfigurator)
                .Update(update => update.Action("Update", _controllerName).Data("getUpdatedOriginValues"))
                .Create("Add", _controllerName)
                .Events(e => { e.RequestEnd("gridDatasourceRequestEnded"); e.Sync("gridDataSourceSynced"); e.Error("gridDatasourceRequestError"); })
            )
            .ToolBar(c =>
            {
                if (_gridModel.EnableFunctionForQuery)
                {
                    c.Custom().Name("commonQuery").Text("查询").HtmlAttributes(new { onclick = "showCommonQueryWindow()", href = "javascript:void(0);" });
                }
                if (_gridModel.EnableFunctionForAdd)
                {
                    c.Create().Text("增加");
                }
                if (_gridModel.EnableFunctionForSave)
                {
                    c.Save();

                }
                if (_gridModel.EnableFunctionForDelete)
                {
                    c.Custom().Name("batchDelete").Text("删除").HtmlAttributes(new { onclick = "batchDelete('" + _gridModel.GridControlId + "','" + _gridModel.KeyColumnName + "','" + _url.Action("BatchDelete", _controllerName) + "')", href = "javascript:void(0);" });
                }
                if (_gridModel.EnableFunctionForEnable)
                {
                    c.Custom().Name("enable").Text("启用").HtmlAttributes(new { onclick = "enable('" + _gridModel.GridControlId + "','" + _gridModel.KeyColumnName + "','" + _url.Action("Enable", _controllerName) + "')", href = "javascript:void(0);" });
                }
                if (_gridModel.EnableFunctionForDisable)
                {
                    c.Custom().Name("disable").Text("禁用").HtmlAttributes(new { onclick = "disable('" + _gridModel.GridControlId + "','" + _gridModel.KeyColumnName + "','" + _url.Action("Disable", _controllerName) + "')", href = "javascript:void(0);" });
                }
                if (_gridModel.EnableFunctionForDetail)
                {
                    c.Custom().Name("detail").Text("客人消费明细列表").HtmlAttributes(new { onclick = "detail('" + _gridModel.GridControlId + "','" + _gridModel.KeyColumnName + "','" + _url.Action("Disable", _controllerName) + "')", href = "javascript:void(0);" });
                }
                if (_customToolbar != null)
                {
                    _customToolbar(c);
                }
            })//.ClientRowTemplate(_gridModel.RowTemplate)
            .ToHtmlString());
        }

        private string _controllerName;
        private HtmlHelper _html;
        private bool _Multiple;
        private UrlHelper _url;
        private KendoGridCommonViewModel _gridModel;
        private Action<GridColumnFactory<T>> _columnConfigurator;
        private Action<DataSourceModelDescriptorFactory<T>> _keyConfigurator;
        private Action<GridToolBarCommandFactory<T>> _customToolbar;
        private Action<PageableBuilder> _pageableBuilder;

    }
}
