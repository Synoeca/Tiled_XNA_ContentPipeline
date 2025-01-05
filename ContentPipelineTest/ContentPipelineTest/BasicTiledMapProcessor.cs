using Microsoft.Xna.Framework.Content.Pipeline;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContentPipelineTest
{
    [ContentProcessor(DisplayName = "BasicTiledMapProcessor")]
    public class BasicTiledMapProcessor : ContentProcessor<BasicTiledMapContent, BasicTiledMapContent>
    {
        public override BasicTiledMapContent Process(BasicTiledMapContent input, ContentProcessorContext context)
        {
            return input;
        }
    }
}
