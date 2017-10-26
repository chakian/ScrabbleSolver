//https://wpfscrabble.codeplex.com/

using System;
using System.Windows;
using System.Windows.Input;

namespace WinForm
{
    public partial class MainWindow : Window
    {
        private const string FileExtension = ".xsc";
        private const string Filter = "Scrabble Documents|*" + FileExtension;

        private string _game;
        private string _fileName;
        private bool _isDirty;

        public bool IsDirty
        {
            get { return _isDirty; }
            set
            {
                if (value == IsDirty) return;

                _isDirty = value;
                //NotifyOfPropertyChange(() => IsDirty);
                //NotifyOfPropertyChange(() => CanSaveGame);
            }
        }

        public string Game
        {
            get { return _game; }
            set
            {
                if (value == Game) return;

                //if (_game != null)
                //    _game.StateChanged -= OnGameStateChanged;

                _game = value;

                //if (_game != null)
                //    _game.StateChanged += OnGameStateChanged;

                IsDirty = false;
                //NotifyOfPropertyChange(() => Game);
            }
        }

        public MainWindow()
        {
            InitializeComponent();

            //DisplayName = Resources.Caption_Scrabble;
            //StartNewGame(new[] { "Esin", "John" }).Execute();

            //return Game != null ? StartNewGame(Game.NamesOfPlayers) : SelectPlayers();
        }

        private void txtLetters_KeyUp(object sender, KeyEventArgs e)
        {
            convertLettersUppercase();
        }
        private void convertLettersUppercase()
        {
            txtLetters.Text = txtLetters.Text.ToUpperInvariant();
            txtLetters.SelectionStart = txtLetters.Text.Length;
        }

        private void btnFindWords_Click(object sender, RoutedEventArgs e)
        {
            lstFoundWords.Items.Add("trert");
            lstFoundWords.Items.Add("treter");
            lstFoundWords.Items.Add("dsada");
        }

        void OnGameStateChanged(object sender, EventArgs e)
        {
            IsDirty = true;
        }

        public string FileName
        {
            get { return _fileName; }
            set
            {
                if (value == FileName) return;

                _fileName = value;
                //DisplayName = string.Format(string.IsNullOrWhiteSpace(FileName) ? "{0}" : "{1} - {0}", "Kelimelik", Path.GetFileName(FileName));
            }
        }

        //public IEnumerable<IResult> NewGame()
        //{
        //    return Game != null ? StartNewGame(Game.NamesOfPlayers) : SelectPlayers();
        //}

        //public IEnumerable<IResult> SelectPlayers()
        //{
        //    var task = Show.Dialog<SelectPlayersViewModel>();
        //    task.Dialog.NamesOfPlayers = Game != null ? Game.NamesOfPlayers : null;

        //    yield return task;
        //    yield return new SequentialResult(StartNewGame(task.Dialog.NamesOfPlayers).GetEnumerator());
        //}

        //public IEnumerable<IResult> OpenGame()
        //{
        //    var selectFile = Show.OpenFileDialog();
        //    selectFile.Filter = Filter;
        //    selectFile.DefaultExtension = FileExtension;
        //    selectFile.CheckIfFileExists = true;

        //    yield return selectFile;

        //    var loadGame = new LoadGameTask { FileName = selectFile.FileName };

        //    yield return loadGame;
        //    yield return CreateGameViewModel(loadGame.Game, selectFile.FileName);
        //}

        public bool CanSaveGame
        {
            get { return Game != null && IsDirty; }
        }

        //public IEnumerable<IResult> SaveGame()
        //{
        //    return Save(FileName);
        //}

        public bool CanSaveGameAs
        {
            get { return Game != null; }
        }

        //public IEnumerable<IResult> SaveGameAs()
        //{
        //    return Save(null);
        //}

        //private IEnumerable<IResult> StartNewGame(IEnumerable<string> players)
        //{
        //    return CreateGameViewModel(new Game(new Random().Next(), players)).AsEnumerable();
        //}

        //private IEnumerable<IResult> Save(string fileName)
        //{
        //    if (String.IsNullOrEmpty(fileName))
        //    {
        //        var selectFile = Show.SaveFileDialog();
        //        selectFile.Filter = Filter;
        //        selectFile.DefaultExtension = FileExtension;

        //        if (!String.IsNullOrWhiteSpace(FileName))
        //        {
        //            selectFile.InitialDirectory = Path.GetDirectoryName(FileName);
        //            selectFile.FileName = Path.GetFileName(FileName);
        //        }

        //        yield return selectFile;
        //        fileName = selectFile.FileName;
        //    }

        //    yield return Game.Save(fileName);

        //    FileName = fileName;
        //    IsDirty = false;
        //}

        //private IResult CreateGameViewModel(IGame game, string fileName = null)
        //{
        //    return new DelegatedTask(() =>
        //    {
        //        Game = new GameViewModel(game);
        //        FileName = fileName ?? String.Empty;

        //        return true;
        //    });
        //}
    }
}
