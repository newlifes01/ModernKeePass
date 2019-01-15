using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using ModernKeePass.Annotations;
using ModernKeePassLib;
using ModernKeePassLib.Cryptography;
using ModernKeePassLib.Security;
using Windows.UI;

namespace ModernKeePass.ViewModels
{
    public class EntryItem : INotifyPropertyChanged
    {
        private bool _isDirty = true;

        public PwEntry Entry { get; }

        public GroupItem Parent { get; }

        public bool HasExpired => HasExpirationDate && Entry.ExpiryTime < DateTime.Now;

        public bool HasUrl => !string.IsNullOrEmpty(Url);

        public double PasswordComplexityIndicator => QualityEstimation.EstimatePasswordBits(Password?.ToCharArray());

        public string Name
        {
            get => GetEntryValue(PwDefs.TitleField);
            set
            {
                SetEntryValue(PwDefs.TitleField, new ProtectedString(true, value));
                OnPropertyChanged();
            }
        }

        public string UserName
        {
            get => GetEntryValue(PwDefs.UserNameField);
            set => SetEntryValue(PwDefs.UserNameField, new ProtectedString(true, value));
        }

        public string Password
        {
            get => GetEntryValue(PwDefs.PasswordField);
            set
            {
                SetEntryValue(PwDefs.PasswordField, new ProtectedString(true, value));
                OnPropertyChanged();
            }
        }

        public string Url
        {
            get => GetEntryValue(PwDefs.UrlField);
            set => SetEntryValue(PwDefs.UrlField, new ProtectedString(true, value));
        }

        public string Notes
        {
            get => GetEntryValue(PwDefs.NotesField);
            set => SetEntryValue(PwDefs.NotesField, new ProtectedString(true, value));
        }

        public int IconId
        {
            get
            {
                if (HasExpired) return (int)PwIcon.Expired;
                if (Entry?.IconId != null) return (int)Entry?.IconId;
                return -1;
            }
            set
            {
                HandleBackup();
                Entry.IconId = (PwIcon)value;
            }
        }

        public DateTimeOffset ExpiryDate
        {
            get => new DateTimeOffset(Entry.ExpiryTime.Date);
            set
            {
                if (!HasExpirationDate) return;
                HandleBackup();
                Entry.ExpiryTime = value.DateTime;
            }
        }

        public TimeSpan ExpiryTime
        {
            get => Entry.ExpiryTime.TimeOfDay;
            set
            {
                if (!HasExpirationDate) return;
                HandleBackup();
                Entry.ExpiryTime = Entry.ExpiryTime.Date.Add(value);
            }
        }

        public bool HasExpirationDate
        {
            get => Entry.Expires;
            set
            {
                Entry.Expires = value;
                OnPropertyChanged();
            }
        }

        public Color BackgroundColor
        {
            get => Color.FromArgb(Entry.BackgroundColor.A, Entry.BackgroundColor.R, Entry.BackgroundColor.G, Entry.BackgroundColor.B);
            set
            {
                Entry.BackgroundColor = System.Drawing.Color.FromArgb(value.A, value.R, value.G, value.B);
                OnPropertyChanged();
            }
        }

        public Color ForegroundColor
        {
            get => Color.FromArgb(Entry.ForegroundColor.A, Entry.ForegroundColor.R, Entry.ForegroundColor.G, Entry.ForegroundColor.B);
            set
            {
                Entry.ForegroundColor = System.Drawing.Color.FromArgb(value.A, value.R, value.G, value.B);
                OnPropertyChanged();
            }
        }

        public IEnumerable<EntryItem> History
        {
            get
            {
                var history = new Stack<EntryItem>();
                foreach (var historyEntry in Entry.History)
                {
                    history.Push(new EntryItem(historyEntry, Parent));
                }
                history.Push(this);

                return history;
            }
        }
        public IEnumerable<string> AdditionalFields
        {
            get
            {
                return Entry.Strings.Select(c => c.Key).Except(PwDefs.GetStandardFields());
            }
        }

        public EntryItem(PwEntry entry, GroupItem parentGroup)
        {
            Entry = entry;
            Parent = parentGroup;
        }

        private void HandleBackup()
        {
            if (_isDirty) return;
            Entry?.Touch(true);
            Entry?.CreateBackup(null);
            _isDirty = true;
        }

        private string GetEntryValue(string key) => Entry?.Strings.GetSafe(key).ReadString();

        private void SetEntryValue(string key, ProtectedString newValue)
        {
            HandleBackup();
            Entry?.Strings.Set(key, newValue);
        }

        public override string ToString() => Entry.LastModificationTime.ToString("g");

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}