namespace ProcessLayer.Entities
{
    public class Deduction : Lookup<int>
    {
        public int? WhenToDeduct { get; set; }
        public bool? ComputedThruSalary { get; set; } = false;
        public bool? GovernmentDeduction { get; set; } = false;
        public bool? AutoCompute { get; set; } = false;
        public WhenToDeduct Deduct { get; set; }
    }
}
