using System.IO;
using System.Text;
using Tridion.ContentManager.CommunicationManagement;
using Tridion.ContentManager.ContentManagement;
using Tridion.ContentManager.Templating;
using Tridion.ContentManager.Templating.Assembly;
using Yahoo.Yui.Compressor;

// Adjust the below target folder.
// [assembly: TcmTargetFolder("/webdav/Adjust%20This%20Target%20Folder")]
// Modified
[assembly: TcmTemplateTitle("TridionMinifier - Assembly")]
namespace TridionMinifier
{
	[TcmTemplateTitle("TridionMinifier - TBB")]
	public class Minifier : ITemplate
	{
		// Which structure group the asset should be published.
		private const string STRUCTURE_GROUP_FOR_CSS = "/webdav/Adjust%20This%20Structure%20Group";

		void ITemplate.Transform(Engine engine, Package package)
		{
			TemplatingLogger logger = TemplatingLogger.GetLogger(this.GetType());

			Item item = package.GetByType(ContentType.Component);

			if (item.Properties.ContainsKey(Item.ItemPropertyTcmUri))
			{
				Component component = (Component)engine.GetObject(item.Properties[Item.ItemPropertyTcmUri].ToString());

				if (component.ComponentType == ComponentType.Multimedia)
				{
					if (string.Compare(component.BinaryContent.MultimediaType.MimeType, "text/css", true) == 0)
					{
						StructureGroup structureGroup = (StructureGroup)engine.GetObject(STRUCTURE_GROUP_FOR_CSS);

						string[] filenameParts = component.BinaryContent.Filename.Split(new char[]{'\\'});
						string filename = filenameParts[filenameParts.GetUpperBound(0)];

						string unMinifiedString = Encoding.Default.GetString(component.BinaryContent.GetByteArray());
						string minifiedString = CssCompressor.Compress(unMinifiedString, 0, CssCompressionType.Hybrid);

						using (MemoryStream memoryStream = new MemoryStream(Encoding.Default.GetBytes(minifiedString)))
						{
							engine.PublishingContext.RenderedItem.AddBinary(
								memoryStream,
								filename,
								structureGroup,
								"mmbyname",
								component,
								component.BinaryContent.MultimediaType.MimeType
							);
						}
					}
				}
			}
		}
	}
}