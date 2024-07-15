namespace Practice {

    public abstract class Creature<Tid, Tstatus> where Tstatus : Enum {
        public Tid Id { get; set; }
        public string Name { get; set; }
        public Tstatus Status { get; set; }
        public DateTime DateOfBirth { get; set; }
    }

    public enum Status {
        Worker = 0,
        NotWorker = 1
    }

    public class Person: Creature<int, Status> {

        public string Email { get; set; }

        public Person(int id, string name, Status status, DateTime dateOfBirth, string email) {
            if (string.IsNullOrWhiteSpace(name)) {
                throw new ArgumentNullException(nameof(name));
            }
            Id = id;
            Name = name;
            Status = status;
            DateOfBirth = dateOfBirth.ToUniversalTime();
            Email = email;
        }
    }
}