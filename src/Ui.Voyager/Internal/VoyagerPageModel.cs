using System.IO;
using System.Reflection;
using System.Text;

namespace GraphQL.Server.Ui.Voyager.Internal
{
    // https://docs.microsoft.com/en-us/aspnet/core/mvc/razor-pages/?tabs=netcore-cli
    internal class VoyagerPageModel
    {
        private string voyagerCSHtml;

        private readonly GraphQLVoyagerOptions settings;

        public VoyagerPageModel(GraphQLVoyagerOptions settings)
        {
            this.settings = settings;
        }

        public string Render()
        {
            if (voyagerCSHtml != null)
            {
                return voyagerCSHtml;
            }

            var assembly = typeof(VoyagerPageModel).GetTypeInfo().Assembly;

            using (var manifestResourceStream = assembly.GetManifestResourceStream("GraphQL.Server.Ui.Voyager.Internal.voyager.cshtml"))
            {
                using (var streamReader = new StreamReader(manifestResourceStream))
                {
                    var builder = new StringBuilder(streamReader.ReadToEnd());
                    builder.Replace("@Model.GraphQLEndPoint", this.settings.GraphQLEndPoint);
                    builder.Replace("@Model.VoyagerPath", this.settings.Path);
                    voyagerCSHtml = builder.ToString();
                    return this.Render();
                }
            }
        }
    }
}
