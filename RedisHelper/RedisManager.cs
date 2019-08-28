using ServiceStack.Redis;
using ServiceStack.Redis.Generic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedisHelper
{
        public class RedisManager
        {
            private static PooledRedisClientManager prcm;

            /// <summary>
            /// 创建链接池管理对象
            /// </summary>
            private static void CreateManager()
            {
                string[] writeServerList = SplitString(RedisConfigInfo.WriteServerList, ",");
                string[] readServerList = SplitString(RedisConfigInfo.ReadServerList, ",");

                prcm = new PooledRedisClientManager(readServerList, writeServerList,
                                 new RedisClientManagerConfig
                                 {
                                     DefaultDb = RedisConfigInfo.DefaultDb,
                                     MaxWritePoolSize = RedisConfigInfo.MaxWritePoolSize,
                                     MaxReadPoolSize = RedisConfigInfo.MaxReadPoolSize,
                                     AutoStart = RedisConfigInfo.AutoStart,
                                 });
            }

            private static string[] SplitString(string strSource, string split)
            {
                return strSource.Split(split.ToArray());
            }

            /// <summary>
            /// 客户端缓存操作对象
            /// </summary>
            public static IRedisClient GetClient()
            {
                if (prcm == null)
                    CreateManager();

                return prcm.GetClient();
            }
            /// <summary>
            /// 缓存默认24小时过期
            /// </summary>
            public static TimeSpan expiresIn = TimeSpan.FromHours(24);
            /// <summary>
            /// 设置一个键值对，默认24小时过期
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="key"></param>
            /// <param name="value"></param>
            /// <param name="redisClient"></param>
            /// <returns></returns>
            public static bool Set<T>(string key, T value, IRedisClient redisClient)
            {

                return redisClient.Set<T>(key, value, expiresIn);
            }

            /// <summary>
            /// 将某类数据插入到list中
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="key">一般是BiaoDiGuid</param>
            /// <param name="item"></param>
            /// <param name="redisClient"></param>
            public static void Add2List<T>(string key, T item, IRedisClient redisClient)
            {
                var redis = redisClient.As<T>();
                var list = redis.Lists[GetListKey(key)];
                list.Add(item);
            }

            /// <summary>
            /// 获取一个list
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="key"></param>
            /// <param name="redisClient"></param>
            /// <returns></returns>
            public static IRedisList<T> GetList<T>(string key, IRedisClient redisClient)
            {
                var redis = redisClient.As<T>();
                return redis.Lists[GetListKey(key)];
            }

            public static string GetListKey(string key, string prefix = null)
            {
                if (string.IsNullOrEmpty(prefix))
                {
                    return "urn:" + key;
                }
                else
                {
                    return "urn:" + prefix + ":" + key;
                }
            }
        }
    
}
