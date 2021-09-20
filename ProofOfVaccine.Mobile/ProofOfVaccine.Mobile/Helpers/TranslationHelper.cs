using System;
using System.Collections.Generic;
using System.Resources;
using System.Reflection;
using System.Text;
using System.Threading;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ProofOfVaccine.Mobile.Helpers
{
    [ContentProperty("Text")]
    public class TranslationHelper : IMarkupExtension
    {
        private const string _resourceId = "ProofOfVaccine.Mobile.AppResources.TextResources";

        private static readonly Lazy<ResourceManager> _manager = new Lazy<ResourceManager>(() =>
            new ResourceManager(_resourceId, typeof(TranslationHelper).GetTypeInfo().Assembly));

        public string Text { get; set; }

        public object ProvideValue(IServiceProvider serviceProvider)
        {
            if (Text == null) return "";

            var currentCI = Thread.CurrentThread.CurrentUICulture;
            var translation = _manager.Value.GetString(Text, currentCI);

            if (translation == null)
            {

#if DEBUG
                throw new ArgumentException(
                    String.Format("Key '{0}' was not found in resources '{1}' for culture '{2}'.", Text, _resourceId, currentCI.Name),
                    "Text");
#else
				translation = Text;
#endif
            }
            return translation;
        }
    }
}

