/*
   ����ͼ��MainWindow.xaml
   �ļ�˵����������ҳ���Ĺ���, ���˵�, ���Ͻǹ�����,TabControl���Open/Close
*/

using GalaSoft.MvvmLight;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using ZFSDomain.Common.CoreLib;
using ZFSDomain.ViewModel.Step;
using ZFSInterface.User;
using System.Linq;
using ZFSData;
using GalaSoft.MvvmLight.Command;
using System.Windows.Controls;
using ZFSDomain.Common.UserControls;
using System;
using ZFSData.InterFace.User;
using System.Threading;
using System.Threading.Tasks;
using ZFSDomain.Common.CoreLib.Helper;
using ZFSDomain.Common.UserControls.Common;
using ZFSDomain.SysModule;
using GalaSoft.MvvmLight.Messaging;
using ZFSDomain.View.User;
using ZFS.Library.Helper;
using ZFS.ILayer.Base;

namespace ZFSDomain.ViewModel
{
    /// <summary>
    /// ��ҳ
    /// </summary>
    public class MainViewModel : BaseHostDialogOperation
    {
        #region ģ��ϵͳ

        private ModuleManager _ModuleManager;

        public ObservableCollection<PageInfo> OpenPageCollection { get; set; } = new ObservableCollection<PageInfo>();

        /// <summary>
        /// ģ�������
        /// </summary>
        public ModuleManager ModuleManager
        {
            get { return _ModuleManager; }
        }

        #endregion

        #region ������

        private PopBoxViewModel _PopBoxView;

        /// <summary>
        /// ��������
        /// </summary>
        public PopBoxViewModel PopBoxView
        {
            get { return _PopBoxView; }
        }

        private NoticeViewModel _NoticeView;

        /// <summary>
        /// ֪ͨģ��
        /// </summary>
        public NoticeViewModel NoticeView
        {
            get { return _NoticeView; }
        }

        #endregion

        #region ����(Binding Command)

        private object _CurrentPage;

        /// <summary>
        /// ��ǰѡ��ҳ
        /// </summary>
        public object CurrentPage
        {
            get { return _CurrentPage; }
            set { _CurrentPage = value; RaisePropertyChanged(); }
        }

        private RelayCommand<Module> _ExcuteCommand;
        private RelayCommand<PageInfo> _ExitCommand;

        /// <summary>
        /// ��ҳ
        /// </summary>
        public RelayCommand<Module> ExcuteCommand
        {
            get
            {
                if (_ExcuteCommand == null)
                {
                    _ExcuteCommand = new RelayCommand<Module>(t => Excute(t));
                }
                return _ExcuteCommand;
            }
            set { _ExcuteCommand = value; RaisePropertyChanged(); }
        }

        /// <summary>
        /// �ر�ҳ
        /// </summary>
        public RelayCommand<PageInfo> ExitCommand
        {
            get
            {
                if (_ExitCommand == null)
                {
                    _ExitCommand = new RelayCommand<PageInfo>(t => ExitPage(t));
                }
                return _ExitCommand;
            }
            set { _ExitCommand = value; RaisePropertyChanged(); }
        }

        #endregion

        #region ��ʼ��/ҳ�����

        /// <summary>
        /// ��ʼ����ҳ
        /// </summary>
        public void InitDefaultView()
        {
            //��ʼ��������,֪ͨ����
            _PopBoxView = new PopBoxViewModel();
            _NoticeView = new NoticeViewModel();
            //���ش���ģ��
            _ModuleManager = new ModuleManager();
            _ModuleManager.LoadModules();
            //����ϵͳĬ����ҳ
            var page = OpenPageCollection.FirstOrDefault(t => t.HeaderName.Equals("ϵͳ��ҳ"));
            if (page == null)
            {
                HomeAbout about = new HomeAbout();
                OpenPageCollection.Add(new PageInfo() { HeaderName = "ϵͳ��ҳ", Body = about });
                CurrentPage = about;
            }
        }

        /// <summary>
        /// ִ��ģ��
        /// </summary>
        /// <param name="module"></param>
        private async void Excute(Module module)
        {
            try
            {
                var page = OpenPageCollection.FirstOrDefault(t => t.HeaderName.Equals(module.Name));
                if (page != null) { CurrentPage = page; return; }
                if (module.ModNameSpcae == null)
                {
                    DefaultViewPage defaultViewPage = new DefaultViewPage();
                    OpenPageCollection.Add(new PageInfo() { HeaderName = module.Name, Body = defaultViewPage });
                    CurrentPage = defaultViewPage;
                }
                else
                {
                    await Task.Run(() =>
                   {
                       App.Current.Dispatcher.Invoke(() =>
                      {
                          var ass = System.Reflection.Assembly.GetExecutingAssembly();
                          if (ass.CreateInstance(module.ModNameSpcae) is IModel dialog)
                          {
                              dialog.BindDefaultModel(module.Authorities);
                              OpenPageCollection.Add(new PageInfo() { HeaderName = module.Name, Body = dialog.GetView() });
                          }
                      });
                   });
                }
                CurrentPage = OpenPageCollection[OpenPageCollection.Count - 1];
            }
            catch (Exception ex)
            {
                Msg.Error(ex.Message, false);
            }
            finally
            {
                Messenger.Default.Send(false, "PackUp");
                GC.Collect();
            }
        }

        /// <summary>
        /// �ر�ҳ��
        /// </summary>
        /// <param name="module"></param>
        private void ExitPage(PageInfo module)
        {
            try
            {
                var tab = OpenPageCollection.FirstOrDefault(t => t.HeaderName.Equals(module.HeaderName));
                if (tab.HeaderName != "ϵͳ��ҳ") OpenPageCollection.Remove(tab);
            }
            catch (Exception ex)
            {
                Msg.Error(ex.Message);
            }
        }

        #endregion

        public async void Run()
        {
            int Day = 0; //����ְ
            while (true) //����ѭ��
            {
                if (await DoWork(9, 9)) Day++; //��9��9����˳��, +1��
                else if (!await ICU()) break; //����ʧ��,��������

                if (Day == 6) //˳�����6��ĵĹ�����
                {
                    Day = 0;  //���ù�����
                    Thread.Sleep(24); //������,��Ϣһ��
                }
            }

        }

        public async Task<bool> DoWork(int start, int end)
        {
            return await Task.FromResult<bool>(true);
        }

        public async Task<bool> ICU()
        {
            return await Task.FromResult<bool>(true);
        }
    }
}