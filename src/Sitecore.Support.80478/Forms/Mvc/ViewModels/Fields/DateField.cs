﻿using Sitecore.Diagnostics;
using Sitecore.Forms.Mvc.Attributes;
using Sitecore.Forms.Mvc.TypeConverters;
using Sitecore.Forms.Mvc.ViewModels;
using Sitecore.WFFM.Abstractions.Actions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Web.Mvc;

namespace Sitecore.Support.Forms.Mvc.ViewModels.Fields
{
  public class DateField : ValuedFieldViewModel<string>
  {
    private void InitDate(string marker)
    {
      DateTime? current = string.IsNullOrEmpty(this.Value) ? null : new DateTime?(DateUtil.IsoDateToDateTime(this.Value));
      char ch = marker.ToLower()[0];
      switch (ch)
      {
        case 'd':
          this.InitDays(current);
          return;

        case 'm':
          this.InitMonth(marker, current);
          return;
      }
      if (ch == 'y')
      {
        this.InitYears(marker, current);
      }
    }

    private void InitDays(DateTime? current)
    {
      this.Days.Clear();
      int num = current.HasValue ? DateTime.DaysInMonth(current.Value.Year, current.Value.Month) : 0x1f;
      if (!current.HasValue)
      {
        SelectListItem item = new SelectListItem
        {
          Selected = true,
          Text = string.Empty,
          Value = string.Empty
        };
        this.Days.Add(item);
      }
      for (int i = 1; i <= 0x1f; i++)
      {
        if (i <= num)
        {
          SelectListItem item2 = new SelectListItem
          {
            Selected = current.HasValue && (current.Value.Day == i),
            Text = i.ToString(CultureInfo.InvariantCulture),
            Value = i.ToString(CultureInfo.InvariantCulture)
          };
          this.Days.Add(item2);
        }
      }
    }

    public override void Initialize()
    {
      if (string.IsNullOrEmpty(this.DateFormat))
      {
        this.DateFormat = "yyyy-MMMM-dd";
      }
      if (this.StartDate == DateTime.MinValue)
      {
        this.StartDate = DateUtil.IsoDateToDateTime("20000101T120000");
      }
      if (this.EndDate == DateTime.MinValue)
      {
        this.EndDate = DateTime.Now.AddYears(1).Date;
      }
      this.Years = new List<SelectListItem>();
      this.Months = new List<SelectListItem>();
      this.Days = new List<SelectListItem>();
      this.InitItems();
    }

    private void InitItems()
    {
      List<string> list = new List<string>(this.DateFormat.Split(new char[] { '-' }));
      list.Reverse();
      list.ForEach(new Action<string>(this.InitDate));
    }

    private void InitMonth(string marker, DateTime? current)
    {
      DateTime time = new DateTime();
      this.Months.Clear();
      if (!current.HasValue)
      {
        SelectListItem item = new SelectListItem
        {
          Selected = true,
          Text = string.Empty,
          Value = string.Empty
        };
        this.Months.Add(item);
      }
      for (int i = 1; i <= 12; i++)
      {
        SelectListItem item2 = new SelectListItem
        {
          Selected = current.HasValue && (current.Value.Month == i),
          Text = string.Format("{0:" + marker + "}", time.AddMonths(i - 1)),
          Value = i.ToString(CultureInfo.InvariantCulture)
        };
        this.Months.Add(item2);
      }
    }

    private void InitYears(string marker, DateTime? current)
    {
      DateTime time = new DateTime(this.StartDate.Year - 1, 1, 1);
      this.Years.Clear();
      if (!current.HasValue)
      {
        SelectListItem item = new SelectListItem
        {
          Selected = true,
          Text = string.Empty,
          Value = string.Empty
        };
        this.Years.Add(item);
      }
      for (int i = this.StartDate.Year; i <= this.EndDate.Year; i++)
      {
        time = time.AddYears(1);
        SelectListItem item2 = new SelectListItem
        {
          Text = string.Format("{0:" + marker + "}", time),
          Value = i.ToString(CultureInfo.InvariantCulture),
          Selected = current.HasValue && (current.Value.Year == i)
        };
        this.Years.Add(item2);
      }
    }

    protected void OnValueUpdated()
    {
      //commented out to overcome the problem that occurs when selected date has been set
      //if (!string.IsNullOrEmpty(this.Value))
      //{
      //  DateTime time = DateUtil.IsoDateToDateTime(this.Value);
      //  this.Day = time.Day;
      //  this.Month = time.Month;
      //  this.Year = time.Year;
      //}
      this.InitItems();
    }

    public override ControlResult GetResult()
    {
      try
      {
        string[] days = this.Day as string[];
        string d = days[0];
        if (d.Length == 1)
        {
          d = "0" + d;
        }
        string[] months = this.Month as string[];
        string m = months[0];
        if (m.Length == 1)
        {
          m = "0" + m;
        }
        string[] years = this.Year as string[];
        string y = years[0];
        if (!string.IsNullOrEmpty(d) && !string.IsNullOrEmpty(m) && !string.IsNullOrEmpty(y))
        {
          this.Value = y + m + d + "T000000";
        }
        else
        {
          this.Value = "20000101T000000";
        }
      }
      catch (Exception ex)
      {
        Log.Error("Sitecore.Support.80478 exception in GetResult method: " + ex.Message, this);
      }

      return new ControlResult(base.FieldItemId, this.Title, this.Value, this.ResultParameters, false);
    }

    [DefaultValue("yyyy-MMMM-dd")]
    public string DateFormat { get; set; }

    public object Day { get; set; }

    public List<SelectListItem> Days { get; private set; }

    public string DayTitle { get; set; }

    [TypeConverter(typeof(IsoDateTimeConverter))]
    public DateTime EndDate { get; set; }

    public object Month { get; set; }

    public List<SelectListItem> Months { get; private set; }

    public string MonthTitle { get; set; }

    public override string ResultParameters =>
        this.DateFormat;

    [TypeConverter(typeof(IsoDateTimeConverter))]
    public DateTime StartDate { get; set; }

    [ParameterName("SelectedDate")]
    public override string Value
    {
      get
      {
        return base.Value;
      }
      set
      {
        base.Value = value;
        this.OnValueUpdated();
      }
    }

    public object Year { get; set; }

    public List<SelectListItem> Years { get; private set; }

    public string YearTitle { get; set; }
  }
}
