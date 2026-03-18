import { describe, it, expect } from 'vitest'
import { mount } from '@vue/test-utils'
import { ref } from 'vue'
import { createVuetify } from 'vuetify'
import * as components from 'vuetify/components'
import * as directives from 'vuetify/directives'
import IndexPage from '@/pages/index.vue'

const vuetify = createVuetify({ components, directives })

const mockIsLoggedIn = ref(false)
vi.stubGlobal('useState', vi.fn((key, init) => {
  if (key === 'isLoggedIn') return mockIsLoggedIn
  return ref(init ? init() : null)
}))

describe('IndexPage.vue', () => {
  it('renders the brand title and subtitle', () => {
    const wrapper = mount(IndexPage, {
      global: { 
        plugins: [vuetify],
        stubs: { 'nuxt-link': true }
      }
    })
    
    expect(wrapper.text()).toContain('Seasoned')
    expect(wrapper.text()).toContain('A Recipe Generator')
  })

  it('shows "Get Started" button when NOT logged in', () => {
    const wrapper = mount(IndexPage, {
      global: { plugins: [vuetify] }
    })

    expect(wrapper.text()).toContain('Get Started')
    expect(wrapper.text()).not.toContain('Talk to Chef')
  })

  it('hides "Get Started" and shows action buttons when logged in', async () => {

    const wrapper = mount(IndexPage, {
      global: { plugins: [vuetify] }
    })

    const isLoggedIn = useState('isLoggedIn')
    isLoggedIn.value = true
    
    await wrapper.vm.$nextTick()

    expect(wrapper.text()).toContain('Talk to Chef')
    expect(wrapper.text()).toContain('Go to Uploader')
    expect(wrapper.text()).not.toContain('Get Started')
  })
})