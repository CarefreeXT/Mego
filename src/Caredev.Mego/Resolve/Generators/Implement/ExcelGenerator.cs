// Copyright (c) CarefreeXT and Caredev Studios. All rights reserved.
// Licensed under the GNU Lesser General Public License v3.0.
// See License.txt in the project root for license information.
namespace Caredev.Mego.Resolve.Generators.Implement
{
    /// <summary>
    /// 针对 Excel 数据库的代码生成器。
    /// </summary>
    public abstract class ExcelBaseGenerator : MSLocalDbGenerator
    {
        /// <inheritdoc/>
        public override FragmentWriterBase FragmentWriter
        {
            get
            {
                if (_FragmentWriter == null)
                {
                    _FragmentWriter = new ExcelFragmentWriter(this);
                }
                return _FragmentWriter;
            }
        }
        private FragmentWriterBase _FragmentWriter;
        /// <inheritdoc/>
        public override string ProviderName => "System.Data.Excel";
        /// <inheritdoc/>
        public override DbFeature Feature => _Feature;
        private readonly DbFeature _Feature = new DbFeature()
        {
            Capability = EDbCapable.Schema | EDbCapable.DataDefinition |
                EDbCapable.WindowFunction | EDbCapable.TableValuedFunction |
                EDbCapable.SubQuery
        };
    }
    /// <summary>
    /// 针对 Access 数据库的代码生成器。
    /// </summary>
    public class ExcelGenerator : ExcelBaseGenerator
    {
        /// <inheritdoc/>
        public override short Version => 0x0400;
    }
}
