using Judge0.DotNet;
using Judge0.DotNet.Models.Submissions;


namespace LonelyInterview.CodeExecution
{
    public class CodeExecutionService
    {
        private readonly ISubmissionService _submissionService;

        public CodeExecutionService(ISubmissionService submissionService)
        {
            _submissionService = submissionService;
        }

        public async Task CreateSubmission()
        {
            var submission = new Submission(
                "#include <stdio.h>\n\nint main(void) {\n  char name[10];\n  scanf(\"%s\", name);\n  printf(\"hello, %s\n\", name);\n  return 0;\n}",
                50)
            {
                Stdin = "world"
            };

            var response = await _submissionService.CreateAsync(submission);
        }
    }
}
