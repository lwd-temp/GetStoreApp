﻿using GetStoreApp.Extensions.DataType.Exceptions;
using GetStoreApp.Helpers.Controls.Download;
using GetStoreApp.Helpers.Root;
using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Data.Json;
using Windows.Storage;
using Windows.Web.Http;

namespace GetStoreApp.Services.Controls.Download
{
    /// <summary>
    /// Aria2下载服务
    /// </summary>
    public static class Aria2Service
    {
        private static string Aria2FilePath { get; } = string.Format(@"{0}\{1}", InfoHelper.GetAppInstalledLocation(), @"Mile.Aria2.exe");

        private static string DefaultAria2ConfPath { get; } = string.Format(@"{0}\{1}", InfoHelper.GetAppInstalledLocation(), @"Mile.Aria2.conf");

        public static string Aria2ConfPath { get; } = string.Format(@"{0}\{1}", ApplicationData.Current.LocalFolder.Path, "Aria2.conf");

        private static string Aria2Arguments { get; set; }

        private static string RPCServerLink { get; } = "http://127.0.0.1:6300/jsonrpc";

        private static bool IsAria2ProcessRunning = false;

        /// <summary>
        /// 初始化Aria2配置文件
        /// </summary>
        public static void InitializeAria2Conf()
        {
            try
            {
                // 原配置文件存在且新的配置文件不存在，拷贝到指定目录
                if (File.Exists(DefaultAria2ConfPath) && !File.Exists(Aria2ConfPath))
                {
                    new FileInfo(DefaultAria2ConfPath).CopyTo(Aria2ConfPath);
                }

                // 使用自定义的配置文件目录
                Aria2Arguments = string.Format("--conf-path=\"{0}\" -D", Aria2ConfPath);
            }

            //  发生异常时，使用默认的配置文件目录
            catch (Exception)
            {
                if (File.Exists(DefaultAria2ConfPath))
                {
                    Aria2Arguments = string.Format("--conf-path=\"{0}\" -D", DefaultAria2ConfPath);
                }
            }
        }

        /// <summary>
        /// 初始化运行Aria2下载进程
        /// </summary>
        public static void StartAria2Process()
        {
            IsAria2ProcessRunning = Aria2ProcessHelper.RunAria2Process(Aria2FilePath, Aria2Arguments);
        }

        /// <summary>
        /// 关闭Aria2进程
        /// </summary>
        public static void CloseAria2()
        {
            if (IsAria2ProcessRunning)
            {
                Aria2ProcessHelper.KillAria2Process();
            }
        }

        /// <summary>
        /// 恢复配置文件默认值
        /// </summary>
        public static void RestoreDefault()
        {
            try
            {
                // 原配置文件存在时，覆盖已经修改的配置文件
                if (File.Exists(DefaultAria2ConfPath))
                {
                    new FileInfo(DefaultAria2ConfPath).CopyTo(Aria2ConfPath, true);
                }

                // 使用自定义的配置文件目录
                Aria2Arguments = string.Format("--conf-path=\"{0}\" -D", Aria2ConfPath);
            }

            //  发生异常时，使用默认的配置文件目录
            catch (Exception)
            {
                if (File.Exists(DefaultAria2ConfPath))
                {
                    Aria2Arguments = string.Format("--conf-path=\"{0}\" -D", DefaultAria2ConfPath);
                }
            }
        }

        /// <summary>
        /// 添加下载任务
        /// </summary>
        public static async Task<(bool, string)> AddUriAsync(string downloadLink, string folderPath)
        {
            // 添加超时设置（半分钟后停止获取）
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
            cancellationTokenSource.CancelAfter(TimeSpan.FromSeconds(30));

            try
            {
                // 判断下载进程是否存在
                if (!Aria2ProcessHelper.IsAria2ProcessExisted())
                {
                    throw new ProcessNotExistException();
                }

                // 创建AddUri Json字符串对象
                JsonObject AddUriObject = new JsonObject();

                // Aria2 请求的消息Id
                AddUriObject["id"] = JsonValue.CreateStringValue(string.Empty);
                // 固定值
                AddUriObject["jsonrpc"] = JsonValue.CreateStringValue("2.0");
                // Aria2 Json RPC对应的方法名
                AddUriObject["method"] = JsonValue.CreateStringValue("aria2.addUri");

                // 创建子参数Json字符串对象。
                JsonObject SubParamObject = new JsonObject();
                SubParamObject["dir"] = JsonValue.CreateStringValue(folderPath);

                // 创建参数数组：
                // 第一个参数为数组，是下载文件的Url
                // 第二个参数是Json字符串对象，成员为下载参数
                JsonArray ParamsArray = new JsonArray()
                {
                    new JsonArray(){ JsonValue.CreateStringValue(downloadLink) },
                    SubParamObject
                };

                AddUriObject["params"] = ParamsArray;

                // 将下载信息转换为Json字符串
                string AddUriString = AddUriObject.Stringify();

                // 使用Aria2 Json RPC接口添加下载任务指令
                byte[] ContentBytes = Encoding.UTF8.GetBytes(AddUriString);

                HttpStringContent httpContent = new HttpStringContent(AddUriString);
                httpContent.Headers.ContentLength = Convert.ToUInt64(ContentBytes.Length);
                httpContent.Headers.ContentType.CharSet = "utf-8";

                HttpClient httpClient = new HttpClient();
                HttpResponseMessage response = await httpClient.PostAsync(new Uri(RPCServerLink), httpContent).AsTask(cancellationTokenSource.Token);

                // 请求成功
                if (response.IsSuccessStatusCode)
                {
                    string ResponseContent = await response.Content.ReadAsStringAsync();

                    JsonObject ResultObject = JsonObject.Parse(ResponseContent);
                    return (true, ResultObject.GetNamedString("result"));
                }
                // 请求失败
                else
                {
                    throw new Exception();
                }
            }
            // 捕捉进程不存在异常
            catch (ProcessNotExistException)
            {
                return (false, string.Empty);
            }
            // 捕捉因访问超时引发的异常
            catch (OperationCanceledException)
            {
                return (false, string.Empty);
            }
            // 其他异常
            catch (Exception)
            {
                return (false, string.Empty);
            }
            finally
            {
                cancellationTokenSource.Dispose();
            }
        }

        /// <summary>
        /// 暂停下载选定的任务
        /// </summary>
        public static async Task<(bool, string)> PauseAsync(string GID)
        {
            // 添加超时设置（半分钟后停止获取）
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
            cancellationTokenSource.CancelAfter(TimeSpan.FromSeconds(30));

            try
            {
                // 判断下载进程是否存在
                if (!Aria2ProcessHelper.IsAria2ProcessExisted())
                {
                    throw new ProcessNotExistException();
                }

                // 创建Pause Json字符串对象
                JsonObject PauseObject = new JsonObject();

                // Aria2 请求的消息Id
                PauseObject["id"] = JsonValue.CreateStringValue(string.Empty);
                // 固定值
                PauseObject["jsonrpc"] = JsonValue.CreateStringValue("2.0");
                // Aria2 Json RPC对应的方法名
                PauseObject["method"] = JsonValue.CreateStringValue("aria2.forceRemove");

                // 创建参数数组
                JsonArray ParamsArray = new JsonArray()
                {
                    JsonValue.CreateStringValue(GID)
                };

                PauseObject["params"] = ParamsArray;

                // 将暂停信息转换为Json字符串
                string PauseString = PauseObject.Stringify();

                // 使用Aria2 Json RPC接口添加暂停下载任务指令
                byte[] ContentBytes = Encoding.UTF8.GetBytes(PauseString);

                HttpStringContent httpContent = new HttpStringContent(PauseString);
                httpContent.Headers.ContentLength = Convert.ToUInt64(ContentBytes.Length);
                httpContent.Headers.ContentType.CharSet = "utf-8";

                HttpClient httpClient = new HttpClient();
                HttpResponseMessage response = await httpClient.PostAsync(new Uri(RPCServerLink), httpContent).AsTask(cancellationTokenSource.Token);

                // 请求成功
                if (response.IsSuccessStatusCode)
                {
                    string ResponseContent = await response.Content.ReadAsStringAsync();

                    JsonObject ResultObject = JsonObject.Parse(ResponseContent);
                    return (true, ResultObject.GetNamedString("result"));
                }
                // 请求失败
                else
                {
                    throw new Exception();
                }
            }
            // 捕捉进程不存在异常
            catch (ProcessNotExistException)
            {
                return (false, string.Empty);
            }
            // 捕捉因访问超时引发的异常
            catch (OperationCanceledException)
            {
                return (false, string.Empty);
            }
            // 其他异常
            catch (Exception)
            {
                return (false, string.Empty);
            }
            finally
            {
                cancellationTokenSource.Dispose();
            }
        }

        /// <summary>
        /// 取消下载选定的任务
        /// </summary>
        public static async Task<(bool, string)> DeleteAsync(string GID)
        {
            // 添加超时设置（半分钟后停止获取）
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
            cancellationTokenSource.CancelAfter(TimeSpan.FromSeconds(30));

            try
            {
                // 判断下载进程是否存在
                if (!Aria2ProcessHelper.IsAria2ProcessExisted())
                {
                    throw new ProcessNotExistException();
                }

                // 创建Delete Json字符串对象
                JsonObject DeleteObject = new JsonObject();

                // Aria2 请求的消息Id
                DeleteObject["id"] = JsonValue.CreateStringValue(string.Empty);
                // 固定值
                DeleteObject["jsonrpc"] = JsonValue.CreateStringValue("2.0");
                // Aria2 Json RPC对应的方法名
                DeleteObject["method"] = JsonValue.CreateStringValue("aria2.forceRemove");

                // 创建参数数组
                JsonArray ParamsArray = new JsonArray()
                {
                    JsonValue.CreateStringValue(GID)
                };

                DeleteObject["params"] = ParamsArray;

                // 将删除信息转换为Json字符串
                string DeleteString = DeleteObject.Stringify();

                // 使用Aria2 Json RPC接口添加暂停下载任务指令
                byte[] ContentBytes = Encoding.UTF8.GetBytes(DeleteString);

                HttpStringContent httpContent = new HttpStringContent(DeleteString);
                httpContent.Headers.ContentLength = Convert.ToUInt64(ContentBytes.Length);
                httpContent.Headers.ContentType.CharSet = "utf-8";

                HttpClient httpClient = new HttpClient();
                HttpResponseMessage response = await httpClient.PostAsync(new Uri(RPCServerLink), httpContent).AsTask(cancellationTokenSource.Token);

                // 请求成功
                if (response.IsSuccessStatusCode)
                {
                    string ResponseContent = await response.Content.ReadAsStringAsync();

                    JsonObject ResultObject = JsonObject.Parse(ResponseContent);
                    return (true, ResultObject.GetNamedString("result"));
                }
                // 请求失败
                else
                {
                    throw new Exception();
                }
            }
            // 捕捉进程不存在异常
            catch (ProcessNotExistException)
            {
                return (false, string.Empty);
            }
            // 捕捉因访问超时引发的异常
            catch (OperationCanceledException)
            {
                return (false, string.Empty);
            }
            // 其他异常
            catch (Exception)
            {
                return (false, string.Empty);
            }
            finally
            {
                cancellationTokenSource.Dispose();
            }
        }

        /// <summary>
        /// 汇报下载任务状态信息
        /// </summary>
        public static async Task<(bool, string, double, double, double)> TellStatusAsync(string GID)
        {
            // 添加超时设置（半分钟后停止获取）
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
            cancellationTokenSource.CancelAfter(TimeSpan.FromSeconds(30));

            try
            {
                // 判断下载进程是否存在
                if (!Aria2ProcessHelper.IsAria2ProcessExisted())
                {
                    throw new ProcessNotExistException();
                }

                // 创建TellStatus Json字符串对象
                JsonObject TellStatusObject = new JsonObject();

                // Aria2 请求的消息Id
                TellStatusObject["id"] = JsonValue.CreateStringValue(string.Empty);
                // 固定值
                TellStatusObject["jsonrpc"] = JsonValue.CreateStringValue("2.0");
                // Aria2 Json RPC对应的方法名
                TellStatusObject["method"] = JsonValue.CreateStringValue("aria2.tellStatus");

                // 创建子参数数组
                JsonArray SubParamArray = new JsonArray()
                {
                    JsonValue.CreateStringValue("gid"),
                    JsonValue.CreateStringValue("status"),
                    JsonValue.CreateStringValue("totalLength"),
                    JsonValue.CreateStringValue("completedLength"),
                    JsonValue.CreateStringValue("downloadSpeed")
                };

                // 创建参数数组
                // 第一个参数为数组，是要获取信息状态的Gid
                // 第二个参数为数组，是获取的内容名称
                JsonArray ParamsArray = new JsonArray()
                {
                    JsonValue.CreateStringValue(GID),
                    SubParamArray
                };

                TellStatusObject["params"] = ParamsArray;

                // 将获取状态信息转换为Json字符串
                string TellStatusString = TellStatusObject.Stringify();

                // 使用Aria2 Json RPC接口添加获取状态指令
                byte[] ContentBytes = Encoding.UTF8.GetBytes(TellStatusString);

                HttpStringContent httpContent = new HttpStringContent(TellStatusString);
                httpContent.Headers.ContentLength = Convert.ToUInt64(ContentBytes.Length);
                httpContent.Headers.ContentType.CharSet = "utf-8";

                HttpClient httpClient = new HttpClient();
                HttpResponseMessage response = await httpClient.PostAsync(new Uri(RPCServerLink), httpContent).AsTask(cancellationTokenSource.Token);

                // 请求成功
                if (response.IsSuccessStatusCode)
                {
                    string ResponseContent = await response.Content.ReadAsStringAsync();

                    JsonObject ResultObject = JsonObject.Parse(ResponseContent);
                    JsonObject DownloadResultObject = ResultObject.GetNamedObject("result");

                    return (true,
                        DownloadResultObject.GetNamedString("status"),
                        Convert.ToDouble(DownloadResultObject.GetNamedString("completedLength")),
                        Convert.ToDouble(DownloadResultObject.GetNamedString("totalLength")),
                        Convert.ToDouble(DownloadResultObject.GetNamedString("downloadSpeed"))
                        );
                }
                // 请求失败
                else
                {
                    throw new Exception();
                }
            }
            // 捕捉进程不存在异常
            catch (ProcessNotExistException)
            {
                return (false, string.Empty, default(double), default(double), default(double));
            }
            // 捕捉因访问超时引发的异常
            catch (OperationCanceledException)
            {
                return (false, string.Empty, default(double), default(double), default(double));
            }
            // 其他异常
            catch (Exception)
            {
                return (false, string.Empty, default(double), default(double), default(double));
            }
            finally
            {
                cancellationTokenSource.Dispose();
            }
        }
    }
}
