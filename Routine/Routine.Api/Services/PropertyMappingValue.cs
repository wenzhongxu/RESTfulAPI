using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Routine.Api.Services
{
    public class PropertyMappingValue
    {
        /// <summary>
        /// 映射字段 一个排序字段可以映射到多个目标实体字段上
        /// </summary>
        public IEnumerable<string> DestinationProperties { get; set; }

        /// <summary>
        /// 是否映射反转  比如 Age升序排序 实际是按照出生日期 降序排序
        /// </summary>
        public bool Revert { get; set; }

        public PropertyMappingValue(IEnumerable<string> destinationProperties, bool revert = false)
        {
            DestinationProperties = destinationProperties ?? throw new ArgumentNullException(nameof(destinationProperties));
            Revert = revert;
        }
    }
}
