export class TaskDto {
    idTask: number
    iid: number
    title: string
    description: string
    issue_type: string = 'issue'
    labels: any = []
    due_date: Date = null
    assignee_id: number
    taskName: string
    time_estimate: string
    total_time_spent: string
    status: number
    createUser: number
}
