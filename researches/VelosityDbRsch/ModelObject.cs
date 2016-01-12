using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VelocityDb;

namespace VelosityDbRsch
{
	public class ModelObject : OptimizedPersistable  
	{
		public DateTime LastCheckeDateTime {
			get
			{
				return _lastCheckeDateTime;
			}
			set
			{
				Update();
				_lastCheckeDateTime = value;
			}
		}

		public string Content
		{
			get
			{
				return _content;
			}
			set
			{
				Update();
				_content = value;
			}
		}

		private DateTime _lastCheckeDateTime;
		private string _content;
	}
}
