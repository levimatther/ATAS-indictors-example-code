namespace ATAS.Indicators.Technical
{
	using System;
	using System.Collections.Generic;
	using System.ComponentModel;
	using System.ComponentModel.DataAnnotations;
	using System.Drawing;

	using ATAS.Indicators.Technical.Properties;

	using OFT.Attributes;
	using OFT.Rendering.Context;
	using OFT.Rendering.Tools;

	[DisplayName("Current price")]
	[HelpLink("https://support.atas.net/knowledge-bases/2/articles/49346-current-price")]
	public class CurrentPrice : Indicator
	{
		#region Fields

		private Color _background = Color.Blue;
		private RenderFont _font = new("Roboto", 14);

		private RenderStringFormat _stringFormat = new()
			{ LineAlignment = StringAlignment.Center, Alignment = StringAlignment.Far };

		private Color _textColor = Color.LightBlue;

		#endregion

		#region Properties

		[Display(ResourceType = typeof(Resources), Name = "BackGround")]
		public System.Windows.Media.Color Background
		{
			get => _background.Convert();
			set => _background = value.Convert();
		}

		[Display(ResourceType = typeof(Resources), Name = "TextColor")]
		public System.Windows.Media.Color TextColor
		{
			get => _textColor.Convert();
			set => _textColor = value.Convert();
		}

		[Display(ResourceType = typeof(Resources), Name = "FontSize")]
		[Range(1,30)]
		public float FontSize
		{
			get => _font.Size;
			set
			{
				if (value < 5)
					return;

				_font = new RenderFont("Roboto", value);
			}
		}

		[Display(ResourceType = typeof(Resources), Name = "Show", GroupName = "Time")]
		public bool ShowTime { get; set; } = true;

		[Display(ResourceType = typeof(Resources), Name = "TimeFormat", GroupName = "Time")]
		public string TimeFormat { get; set; } = "HH:mm:ss";

		#endregion

		#region ctor

		public CurrentPrice()
			: base(true)
		{
			SubscribeToDrawingEvents(DrawingLayouts.Final);
			EnableCustomDrawing = true;
			DataSeries[0].IsHidden = true;
			((ValueDataSeries)DataSeries[0]).VisualType = VisualMode.Hide;
			DenyToChangePanel = true;
		}

		#endregion

		#region Protected methods
		
		protected override void OnCalculate(int bar, decimal value)
		{
		}
		
		protected override void OnRender(RenderContext context, DrawingLayouts layout)
		{
			if (LastVisibleBarNumber != CurrentBar - 1 || LastVisibleBarNumber < 0)
				return;

			var candle = GetCandle(LastVisibleBarNumber);
			var priceString = candle.Close.ToString();
			var size = context.MeasureString(priceString, _font);

			var x = (int)(ChartInfo.GetXByBar(LastVisibleBarNumber) + ChartInfo.PriceChartContainer.BarsWidth);
			var y = ChartInfo.GetYByPrice(candle.Close, false);
			var rectangle = new Rectangle(x + 10, y - size.Height / 2, size.Width + 10, size.Height);

			var points = new List<Point>
			{
				new(x, y),
				new(rectangle.X, rectangle.Y),
				new(rectangle.X + rectangle.Width, rectangle.Y),
				new(rectangle.X + rectangle.Width, rectangle.Y + rectangle.Height),
				new(rectangle.X, rectangle.Y + rectangle.Height)
			};

			context.FillPolygon(_background, points.ToArray());

			rectangle.Y++;
			context.DrawString(priceString, _font, _textColor, rectangle, _stringFormat);

			if (ShowTime)
			{
				var time = DateTime.Now.ToString(TimeFormat);
				size = context.MeasureString(time, _font);
				context.DrawString(time, _font, _textColor, rectangle.X + rectangle.Width - size.Width, rectangle.Y - size.Height);
			}
		}

		#endregion
	}
}