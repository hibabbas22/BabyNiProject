$(function () {

  var startDate = $("#startDate").kendoDateTimePicker();
  var endDate = $("#endDate").kendoDateTimePicker();

  $('#daily').click(function () {
    if ($('#daily').is(':checked')) {
      $("#chartHourly").hide();
      $("#gridHourly").hide();
      $("#chartDaily").show();
      $("#gridDaily").show();
      var dataSource = new kendo.data.DataSource({
        transport: {
          read: {
            url: "https://localhost:44319/api/SendDaily/getdata",
            dataType: "json",
            type: "POST",
            data: { "startDate": startDate.val(), "endDate": endDate.val() }
          }
        },
        pageSize: 5
      });


      $("#gridDaily").kendoGrid({
        dataSource: dataSource,
        pageable: true,
        schema: {
          model: {
            fields: {
              TimeKey_daily: { type: "date" },
              Link: { type: "string" },
              Slot: { type: "string" },
              Max_Rx_Level_daily: { type: "double" },
              Max_Tx_Level_daily: { type: "double" },
              Rsl_Deviation_daily: { type: "double" }
            }
          }
        },
        columns: [{
          field: "TimeKey_daily",
          title: "TimeKey_daily",
          format: "{0:MM/dd/yyyy}"
        }, {
          field: "Link",
          title: "Link"
        }, {
          field: "Slot",
          title: "Slot"
        }, {
          field: "Max_Rx_Level_daily",
          title: "Max_Rx_Level_daily"
        }, {
          field: "Max_Tx_Level_daily",
          title: "Max_Tx_Level_daily"
        }, {
          field: "Rsl_Deviation_daily",
          title: "Rsl_Deviation_daily"
        }
        ]
      });
      $("#chartDaily").kendoChart({
        dataSource: dataSource,
        title: {
          text: "Daily Aggregation"
        },
        legend: {
          position: "bottom"
        },
        seriesDefaults: {
          type: "line"
        },
        series: [{
          name: "Max_Rx_Level_daily",
          data: "Max_Rx_Level_daily",
          field: "Max_Rx_Level_daily"
        }, {
          name: "Max_Tx_Level_daily",
          data: "Max_Tx_Level_daily",
          field: "Max_Tx_Level_daily"
        }, {
          name: "Rsl_Deviation_daily",
          data: "Rsl_Deviation_daily",
          field: "Rsl_Deviation_daily"
        }],
        valueAxis: {
          labels: {
            format: "{0}"
          }
        },
        categoryAxis: {
          field: "DateLink"
        }
      });

    }
  });
  $('#hourly').click(function () {
    if ($('#hourly').is(':checked')) {
      $("#chartDaily").hide();
      $("#gridDaily").hide();
      $("#chartHourly").show();
      $("#gridHourly").show();
   
      var dataSource = new kendo.data.DataSource({
        transport: {
          read: {
            url: "https://localhost:44319/api/SendHourly/getdata",
            dataType: "json",
            type: "POST",
            data: { "startdate": startDate.val(), "enddate": endDate.val() }
          }
        },
        pageSize:5
      });

      $("#chartHourly").kendoChart({
        dataSource: dataSource,
        title: {
          text: "Hourly Aggregation"
        },
        legend: {
          position: "bottom"
        },
        seriesDefaults: {
          type: "line"
        },
        series: [{
          name: "Max_Rx_Level",
          data: "Max_Rx_Level",
          field: "Max_Rx_Level"
        }, {
          name: "Max_Tx_Level",
          data: "Max_Tx_Level",
          field: "Max_Tx_Level"
        }, {
          name: "Rsl_Deviation",
          data: "Rsl_Deviation",
          field: "Rsl_Deviation"
        }],
        valueAxis: {
          labels: {
            format: "{0}"
          }
        },
        categoryAxis: {
          field: "DateLink"
        }
      });
      $("#gridHourly").kendoGrid({
        dataSource: dataSource,
        pageable: true,
        schema: {
          model: {
            fields: {
              TimeKey: { type: "date" },
              Link: { type: "string" },
              Slot: { type: "string" },
              Max_Rx_Level: { type: "double" },
              Max_Tx_Level: { type: "double" },
              Rsl_Deviation: { type: "double" }
            }
          }
        },
        columns: [{
          field: "TimeKey",
          title: "TimeKey",
          format: "{0:MM/dd/yyyy}"
        }, {
          field: "Link",
          title: "Link"
        }, {
          field: "Slot",
          title: "Slot"
        }, {
          field: "Max_Rx_Level",
          title: "Max_Rx_Level"
        }, {
          field: "Max_Tx_Level",
          title: "Max_Tx_Level"
        }, {
          field: "Rsl_Deviation",
          title: "Rsl_Deviation"
        },
        ]
      });
    }
  });

});
