﻿// © XIV-Tools.
// Licensed under the MIT license.

namespace XivToolsWpf.Controls
{
	using System.ComponentModel;
	using System.Windows;
	using XivToolsWpf.DependencyInjection;
	using XivToolsWpf.DependencyProperties;

	/// <summary>
	/// Interaction logic for Label.xaml.
	/// </summary>
	public partial class TextBlock : System.Windows.Controls.TextBlock
	{
		public static readonly IBind<string> KeyDp = Binder.Register<string, TextBlock>(nameof(Key), OnKeyChanged, BindMode.OneWay);
		public static readonly IBind<string?> ValueDp = Binder.Register<string?, TextBlock>(nameof(Value), OnValueChanged, BindMode.OneWay);
		public static readonly IBind<bool> AllLanguagesDp = Binder.Register<bool, TextBlock>(nameof(AllLanguages), BindMode.OneWay);

		private static ILocaleProvider? localeProvider;

		public TextBlock()
		{
			if (DesignerProperties.GetIsInDesignMode(this))
				return;

			this.Loaded += this.TextBlock_Loaded;
			this.Unloaded += this.TextBlock_Unloaded;

			localeProvider = DependencyFactory.GetDependency<ILocaleProvider>();
		}

		public string? Key { get; set; }

		public string? Value
		{
			get => ValueDp.Get(this);
			set => ValueDp.Set(this, value);
		}

		public bool AllLanguages
		{
			get => AllLanguagesDp.Get(this);
			set => AllLanguagesDp.Set(this, value);
		}

		public static void OnKeyChanged(TextBlock sender, string val)
		{
			sender.Key = val;

			if (localeProvider == null)
				localeProvider = DependencyFactory.GetDependency<ILocaleProvider>();

			if (localeProvider == null)
				return;

			if (!localeProvider.Loaded)
				return;

			sender.LoadString();
		}

		public static void OnValueChanged(TextBlock sender, string? val)
		{
			sender.LoadString();
		}

		private void TextBlock_Loaded(object sender, RoutedEventArgs e)
		{
			if (localeProvider == null)
				return;

			localeProvider.LocaleChanged += this.OnLocaleChanged;

			if (!localeProvider.Loaded)
				return;

			this.LoadString();
		}

		private void TextBlock_Unloaded(object sender, RoutedEventArgs e)
		{
			if (localeProvider == null)
				return;

			localeProvider.LocaleChanged -= this.OnLocaleChanged;

			localeProvider = null;
		}

		private void OnLocaleChanged()
		{
			this.LoadString();
		}

		private void LoadString()
		{
			if (string.IsNullOrEmpty(this.Key))
				return;

			if (localeProvider == null)
				localeProvider = DependencyFactory.GetDependency<ILocaleProvider>();

			if (localeProvider == null)
				return;

			string? val = null;

			if (!DesignerProperties.GetIsInDesignMode(this))
			{
				if (this.Value == null)
				{
					if (this.AllLanguages)
					{
						val = localeProvider.GetStringAllLanguages(this.Key);
					}
					else
					{
						val = localeProvider.GetString(this.Key);
					}
				}
				else
				{
					val = localeProvider.GetStringFormatted(this.Key, this.Value);
				}
			}

			if (string.IsNullOrEmpty(val))
			{
				if (!string.IsNullOrEmpty(this.Text))
					return;

				val = "[" + this.Key + "]";
			}

			this.Text = val;
		}
	}
}