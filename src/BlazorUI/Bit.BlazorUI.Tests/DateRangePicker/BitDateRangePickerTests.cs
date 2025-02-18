﻿using System;
using System.Collections.Generic;
using Bunit;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bit.BlazorUI.Tests.DateRangePicker;

[TestClass]
public class BitDateRangePickerTests : BunitTestContext
{
    [DataTestMethod,
        DataRow(true),
        DataRow(false),
    ]
    public void BitDateRangePickerTest(bool isEnabled)
    {
        var component = RenderComponent<BitDateRangePicker>(parameters =>
        {
            parameters.Add(p => p.IsEnabled, isEnabled);
        });

        var bitDatePicker = component.Find(".bit-dtrp");

        if (isEnabled)
        {
            Assert.IsFalse(bitDatePicker.ClassList.Contains("bit-dis"));
        }
        else
        {
            Assert.IsTrue(bitDatePicker.ClassList.Contains("bit-dis"));
        }
    }

    [DataTestMethod, DataRow("<div>This is labelFragment</div>")]
    public void BitDateRangePickerShouldRenderLabelFragment(string labelTemplate)
    {
        var component = RenderComponent<BitDateRangePicker>(parameters =>
        {
            parameters.Add(p => p.LabelTemplate, labelTemplate);
        });

        var bitDateRangePickerLabelChild = component.Find(".bit-dtrp > label.bit-dtrp-lbl").ChildNodes;
        bitDateRangePickerLabelChild.MarkupMatches(labelTemplate);
    }

    [DataTestMethod, DataRow("go to today text")]
    public void BitDateRangePickerShouldGiveValueToGoToToday(string goToToday)
    {
        var component = RenderComponent<BitDateRangePicker>(parameters =>
        {
            parameters.Add(p => p.GoToToday, goToToday);
            parameters.Add(p => p.IsOpen, true);
        });

        var goToTodayButton = component.Find(".bit-dtrp-gtd-btn");

        Assert.AreEqual(goToToday, goToTodayButton.TextContent);
    }

    [DataTestMethod,
      DataRow(true, 1),
      DataRow(false, 0)
    ]
    public void BitDateRangePickerShouldHandleOnClickEvent(bool isEnabled, int count)
    {
        Context.JSInterop.Mode = JSRuntimeMode.Loose;
        var clickedValue = 0;
        var component = RenderComponent<BitDateRangePicker>(parameters =>
        {
            parameters.Add(p => p.IsEnabled, isEnabled);
            parameters.Add(p => p.OnClick, () => clickedValue++);
        });

        var bitDateRangePickerInput = component.Find(".bit-dtrp-wrp");
        bitDateRangePickerInput.Click();

        Assert.AreEqual(count, clickedValue);
    }

    [DataTestMethod,
      DataRow(true, 1),
      DataRow(false, 0)
    ]
    public void BitDateRangePickerCalendarItemsShouldRespectIsEnabled(bool isEnabled, int count)
    {
        Context.JSInterop.Mode = JSRuntimeMode.Loose;
        var selectedDateValue = 0;
        var component = RenderComponent<BitDateRangePicker>(parameters =>
        {
            parameters.Add(p => p.IsOpen, true);
            parameters.Add(p => p.IsEnabled, isEnabled);
            parameters.Add(p => p.OnSelectDate, () => selectedDateValue++);
        });

        var dateItems = component.FindAll(".bit-dtrp-dbtn");

        Random random = new();
        int randomNumber = random.Next(0, dateItems.Count - 1);
        dateItems[randomNumber].Click();
        Assert.AreEqual(count, selectedDateValue);
    }

    [DataTestMethod]
    public void BitDateRangePickerCalendarSelectTodayDate()
    {
        Context.JSInterop.Mode = JSRuntimeMode.Loose;
        var component = RenderComponent<BitDateRangePicker>(parameters =>
        {
            parameters.Add(p => p.IsOpen, true);
            parameters.Add(p => p.IsEnabled, true);
        });

        Assert.IsNull(component.Instance.Value.StartDate);
        Assert.IsNull(component.Instance.Value.EndDate);

        var today = component.Find(".bit-dtrp-dc-tdy button.bit-dtrp-dbtn");
        today.Click();

        Assert.IsNotNull(component.Instance.Value.StartDate);
        Assert.IsNull(component.Instance.Value.EndDate);
        Assert.AreEqual(component.Instance.Value.StartDate.Value.Date, DateTimeOffset.Now.Date);
        Assert.AreEqual(component.Instance.Value.StartDate.Value.Offset, DateTimeOffset.Now.Offset);
        today.Click();

        Assert.IsNotNull(component.Instance.Value.StartDate);
        Assert.AreEqual(component.Instance.Value.StartDate.Value.Date, DateTimeOffset.Now.Date);
        Assert.AreEqual(component.Instance.Value.StartDate.Value.Offset, DateTimeOffset.Now.Offset);

        Assert.IsNotNull(component.Instance.Value.EndDate);
        Assert.AreEqual(component.Instance.Value.EndDate.Value.Date, DateTimeOffset.Now.Date);
        Assert.AreEqual(component.Instance.Value.EndDate.Value.Offset, DateTimeOffset.Now.Offset);
    }

    [DataTestMethod]
    public void BitDateRangePickerCalendarWithCustomCultureInfo()
    {
        Context.JSInterop.Mode = JSRuntimeMode.Loose;
        var component = RenderComponent<BitDateRangePicker>(parameters =>
        {
            parameters.Add(p => p.IsOpen, true);
            parameters.Add(p => p.Culture, CultureInfoHelper.GetFaIrCultureByFingilishNames());
        });

        var monthButtons = component.FindAll("button.bit-dtrp-rbtn");
        Assert.IsTrue(monthButtons.Count > 0);
        Assert.AreEqual(12, monthButtons.Count);

        var index = 0;
        foreach (var button in monthButtons)
        {
            Assert.AreEqual(button.FirstElementChild.TextContent, component.Instance.Culture.DateTimeFormat.AbbreviatedMonthNames[index++]);
        }
    }

    [DataTestMethod,
        DataRow("DateRangePicker")
    ]
    public void BitDateRangePickerAriaLabelTest(string pickerAriaLabel)
    {
        Context.JSInterop.Mode = JSRuntimeMode.Loose;
        var component = RenderComponent<BitDateRangePicker>(parameters =>
        {
            parameters.Add(p => p.PickerAriaLabel, pickerAriaLabel);
        });

        var bitDateRangePickerCallout = component.Find(".bit-dtrp-mcal");
        var calloutAriaLabel = bitDateRangePickerCallout.GetAttribute("aria-label");

        Assert.AreEqual(pickerAriaLabel, calloutAriaLabel);
    }

    [DataTestMethod,
        DataRow(false),
        DataRow(true)
    ]
    public void BitDateRangePickerShowGoToTodayTest(bool showGoToToday)
    {
        Context.JSInterop.Mode = JSRuntimeMode.Loose;
        var component = RenderComponent<BitDateRangePicker>(parameters =>
        {
            parameters.Add(p => p.ShowGoToToday, showGoToToday);
        });

        var goToTodayBtnElms = component.FindAll(".bit-dtrp-gtd-btn");

        if (showGoToToday)
        {
            Assert.AreEqual(1, goToTodayBtnElms.Count);
        }
        else
        {
            Assert.AreEqual(0, goToTodayBtnElms.Count);
        }
    }

    [DataTestMethod,
        DataRow(false),
        DataRow(true)
    ]
    public void BitDateRangePickerShowCloseButtonTest(bool showCloseButton)
    {
        Context.JSInterop.Mode = JSRuntimeMode.Loose;
        var component = RenderComponent<BitDateRangePicker>(parameters =>
        {
            parameters.Add(p => p.ShowCloseButton, showCloseButton);
        });

        var closeBtnElms = component.FindAll(".bit-dtrp-cbtn");

        if (showCloseButton)
        {
            Assert.AreEqual(1, closeBtnElms.Count);
        }
        else
        {
            Assert.AreEqual(0, closeBtnElms.Count);
        }
    }

    [DataTestMethod,
        DataRow(false),
        DataRow(true)
    ]
    public void BitDateRangePickerHighlightCurrentMonthTest(bool highlightCurrentMonth)
    {
        Context.JSInterop.Mode = JSRuntimeMode.Loose;
        var component = RenderComponent<BitDateRangePicker>(parameters =>
        {
            parameters.Add(p => p.HighlightCurrentMonth, highlightCurrentMonth);
        });

        var currentMonthCells = component.FindAll(".bit-dtrp-crtm");

        if (highlightCurrentMonth)
        {
            Assert.AreEqual(1, currentMonthCells.Count);
        }
        else
        {
            Assert.AreEqual(0, currentMonthCells.Count);
        }
    }

    [DataTestMethod,
        DataRow(false),
        DataRow(true)
    ]
    public void BitDateRangePickerHighlightSelectedMonthTest(bool highlightSelectedMonth)
    {
        Context.JSInterop.Mode = JSRuntimeMode.Loose;
        var component = RenderComponent<BitDateRangePicker>(parameters =>
        {
            parameters.Add(p => p.HighlightSelectedMonth, highlightSelectedMonth);
        });


        var selectedMonthCells = component.FindAll(".bit-dtrp-selm");

        if (highlightSelectedMonth)
        {
            Assert.AreEqual(1, selectedMonthCells.Count);
        }
        else
        {
            Assert.AreEqual(0, selectedMonthCells.Count);
        }
    }

    [DataTestMethod]
    public void BitDateRangePickerCalloutHtmlAttributesTest()
    {
        Context.JSInterop.Mode = JSRuntimeMode.Loose;
        var calloutHtmlAttributes = new Dictionary<string, object>
        {
            {"style", "color: blue" }
        };

        var component = RenderComponent<BitDateRangePicker>(parameters =>
        {
            parameters.Add(p => p.CalloutHtmlAttributes, calloutHtmlAttributes);
        });

        var bitDateRangePickerCallout = component.Find(".bit-dtrp-mcal");
        var calloutStyle = bitDateRangePickerCallout.GetAttribute("style");

        Assert.AreEqual("color: blue", calloutStyle);
    }
}
