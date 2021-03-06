﻿using SIPackages;
using SIPackages.Core;
using SIQuester.Model;
using SIQuester.ViewModel.Properties;
using System;
using System.Windows.Input;

namespace SIQuester.ViewModel
{
    /// <summary>
    /// Создание нового пакета
    /// </summary>
    public sealed class NewViewModel: WorkspaceViewModel
    {
        private PackageType _packageType = PackageType.Classic;
        private string _packageName = "Вопросы SIGame";
        private string _packageAuthor = Environment.UserName;

        /// <summary>
        /// Создать пакет
        /// </summary>
        public ICommand Create { get; }

        public override string Header => "Новый пакет";

        public PackageType PackageType
        {
            get { return _packageType; }
            set
            {
                if (_packageType != value)
                {
                    _packageType = value;
                    OnPropertyChanged();
                }
            }
        }

        public string PackageName
        {
            get { return _packageName; }
            set
            {
                if (_packageName != value)
                {
                    _packageName = value;
                    OnPropertyChanged();
                }
            }
        }

        public string PackageAuthor
        {
            get { return _packageAuthor; }
            set
            {
                if (_packageAuthor != value)
                {
                    _packageAuthor = value;
                    OnPropertyChanged();
                }
            }
        }

        public NonStandartPackageParams PackageParams { get; } = new NonStandartPackageParams();

        private readonly StorageContextViewModel _storageContextViewModel;

        public NewViewModel(StorageContextViewModel storageContextViewModel)
        {
            _storageContextViewModel = storageContextViewModel;

            Create = new SimpleCommand(Create_Executed);
        }

        private void Create_Executed(object arg)
        {
            try
            {
                var doc = SIDocument.Create(_packageName, _packageAuthor);

                switch (_packageType)
                {
                    case PackageType.Classic:
                        {
                            for (int i = 0; i < 3; i++)
                            {
                                var round = doc.Package.CreateRound(RoundTypes.Standart, null);
                                for (int j = 0; j < 6; j++)
                                {
                                    var theme = round.CreateTheme(null);
                                    for (int k = 0; k < 5; k++)
                                        theme.CreateQuestion(100 * (i + 1) * (k + 1));
                                }
                            }
                            var final = doc.Package.CreateRound(RoundTypes.Final, Resources.FinalName);
                            for (int j = 0; j < 7; j++)
                            {
                                final.CreateTheme(null).CreateQuestion(0);
                            }
                        }
                        break;

                    case PackageType.Special:
                        {
                            var param = PackageParams;
                            for (int i = 0; i < param.NumOfRounds; i++)
                            {
                                var round = doc.Package.CreateRound(RoundTypes.Standart, null);
                                for (int j = 0; j < param.NumOfThemes; j++)
                                {
                                    var theme = round.CreateTheme(null);
                                    for (int k = 0; k < param.NumOfQuestions; k++)
                                        theme.CreateQuestion(param.NumOfPoints * (i + 1) * (k + 1));
                                }
                            }
                            if (param.HasFinal)
                            {
                                var final = doc.Package.CreateRound(RoundTypes.Final, Resources.FinalName);
                                for (int j = 0; j < param.NumOfFinalThemes; j++)
                                {
                                    final.CreateTheme(null).CreateQuestion(0);
                                }
                            }
                        }
                        break;

                    case PackageType.ThemesCollection:
                        doc.Package.CreateRound(RoundTypes.Standart, Resources.ThemesCollection);
                        break;

                    case PackageType.Empty:
                    default:
                        break;
                }

                OnNewItem(new QDocument(doc, _storageContextViewModel) { FileName = doc.Package.Name });

                OnClosed();
            }
            catch (Exception exc)
            {
                OnError(exc);
            }
        }
    }
}
