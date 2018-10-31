using System;
using MessagePack;
#if Server
using Message;
using System.Collections.Generic;
using System.Linq;
using Dapper;
using System.Data;
#endif

namespace Common 
{
    [Union(11, typeof(Role))]
    public interface IDBData
    {

    }
	/// <summary>
	/// 数据表实体类：Role 
	/// </summary>
	[MessagePackObject]
	public class Role
	:IDBData
	{    
				 
		/// <summary>
		/// Int64:玩家UID:
		/// </summary>                       
		[Key(0)]
		public Int64 uid {get;set;}   
				 
		/// <summary>
		/// String:名字:
		/// </summary>                       
		[Key(1)]
		public String name {get;set;}   
				 
		/// <summary>
		/// Int16:性别:
		/// </summary>                       
		[Key(2)]
		public Int16 sex {get;set;}   
				 
		/// <summary>
		/// String:头像url:
		/// </summary>                       
		[Key(3)]
		public String head_url {get;set;}   
				 
		/// <summary>
		/// Int16:等级:((1))
		/// </summary>                       
		[Key(4)]
		public Int16 lv {get;set;}   
				 
		/// <summary>
		/// Int32:经验:((0))
		/// </summary>                       
		[Key(5)]
		public Int32 exp {get;set;}   
				 
		/// <summary>
		/// Int32:钻石:((0))
		/// </summary>                       
		[Key(6)]
		public Int32 diamond {get;set;}   
				 
		/// <summary>
		/// Int32:金币:((0))
		/// </summary>                       
		[Key(7)]
		public Int32 gold {get;set;}   
				 
		/// <summary>
		/// Int32:仓库等级:((1))
		/// </summary>                       
		[Key(8)]
		public Int32 repertory_lv {get;set;}   
				 
		/// <summary>
		/// Int32:油:((100000))
		/// </summary>                       
		[Key(9)]
		public Int32 oil {get;set;}   
				 
		/// <summary>
		/// Int32:弹:((100000))
		/// </summary>                       
		[Key(10)]
		public Int32 ammo {get;set;}   
				 
		/// <summary>
		/// Int32:钢:((100000))
		/// </summary>                       
		[Key(11)]
		public Int32 steel {get;set;}   
				 
		/// <summary>
		/// Int32:铝:((100000))
		/// </summary>                       
		[Key(12)]
		public Int32 al {get;set;}   
				 
		/// <summary>
		/// DateTime:资源最后恢复时间:(getdate())
		/// </summary>                       
		#if Server
		[MongoDB.Bson.Serialization.Attributes.BsonDateTimeOptions(Kind = DateTimeKind.Local)]
#endif
		[Key(13)]
		public DateTime recovery_time {get;set;}   
				 
		/// <summary>
		/// Int32:最大副本:((0))
		/// </summary>                       
		[Key(14)]
		public Int32 max_map {get;set;}   
		   
#if Server
		[IgnoreMember]
		public string selectall => @"SELECT * FROM Role WHERE uid=@uid";
		[IgnoreMember]
		public string select => @"SELECT * FROM Role WHERE uid = @uid ";
		[IgnoreMember]
		public string save => @"MERGE INTO Role AS t1
								USING(SELECT @uid as uid,@name as name,@sex as sex,@head_url as head_url,@lv as lv,@exp as exp,@diamond as diamond,@gold as gold,@repertory_lv as repertory_lv,@oil as oil,@ammo as ammo,@steel as steel,@al as al,@recovery_time as recovery_time,@max_map as max_map) as t2
								ON ( t1.uid = t2.uid)
								WHEN MATCHED THEN
									UPDATE SET name = t2.name,sex = t2.sex,head_url = t2.head_url,lv = t2.lv,exp = t2.exp,diamond = t2.diamond,gold = t2.gold,repertory_lv = t2.repertory_lv,oil = t2.oil,ammo = t2.ammo,steel = t2.steel,al = t2.al,recovery_time = t2.recovery_time,max_map = t2.max_map 
                                WHEN NOT MATCHED THEN
									INSERT(uid,name,sex,head_url,lv,exp,diamond,gold,repertory_lv,oil,ammo,steel,al,recovery_time,max_map) VALUES(t2.uid,t2.name,t2.sex,t2.head_url,t2.lv,t2.exp,t2.diamond,t2.gold,t2.repertory_lv,t2.oil,t2.ammo,t2.steel,t2.al,t2.recovery_time,t2.max_map);";
		[IgnoreMember]
        public string delete => "DELETE FROM Role WHERE uid = @uid";
		[IgnoreMember]
		public string cacheKey => "Role:{" + uid.ToString() + "}";
		[IgnoreMember]
        public string cacheField => uid.ToString();
#endif
	}    
 }

