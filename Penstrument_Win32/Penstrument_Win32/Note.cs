namespace Penstrument_Win32
{
    public class Note : ValueType
    {
        readonly NoteLetter note;
        readonly int octave;

        public Note(NoteLetter note, int octave)
        {
            this.note = note;
            this.octave = octave;
        }

        public override string ToString()
        {
            return note.ToString() + octave;
        }

        public byte ToByte()
        {
            return (byte)((octave + 1) * 12 + note);
        }
    }

    public enum NoteLetter
    {
        C = 0, CS, D, DS, E, F, FS, G, GS, A, AS, B
    }
}
